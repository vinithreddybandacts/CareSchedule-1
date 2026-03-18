using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using CareSchedule.Services.Interface;
using CareSchedule.DTOs;

using CareSchedule.Models;
using CareSchedule.Infrastructure.Data;
using CareSchedule.Infrastructure;

using CareSchedule.Repositories.Interface;

namespace CareSchedule.Services.Implementation
{
    public class BookingService(
            CareScheduleContext _db,
            IUnitOfWork _uow,
            IAppointmentRepository _apptRepo,
            IAppointmentChangeRepository _changeRepo,
            IPublishedSlotBookingRepository _slotRepo,
            ICalendarEventRepository _calendarRepo,
            INotificationRepository _notifRepo,
            IReminderScheduleRepository _reminderRepo,
            ISystemConfigRepository _configRepo,
            IAuditLogService _auditService)
            : IBookingService
    {
        public AppointmentResponseDto Book(BookAppointmentRequestDto dto)
        {
            if (dto.PublishedSlotId <= 0) throw new ArgumentException("Invalid PublishedSlotId.");
            if (dto.PatientId <= 0) throw new ArgumentException("Invalid PatientId.");
            if (string.IsNullOrWhiteSpace(dto.BookingChannel)) dto.BookingChannel = "FrontDesk";

            var slot = _slotRepo.GetById(dto.PublishedSlotId);
            if (slot == null) throw new KeyNotFoundException("Slot not found.");
            if (!string.Equals(slot.Status, "Open", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("SLOT_UNAVAILABLE");

            // Optional capacity rule (MVP): read default max/day from SystemConfig (if present)
            var maxPerDay = _configRepo.GetInt("booking.capacity.default.maxPerProviderPerDay", null);
            if (maxPerDay.HasValue)
            {
                var countToday = _apptRepo.CountByProviderDate(slot.ProviderId, slot.SiteId, slot.SlotDate, "Booked");
                if (countToday >= maxPerDay.Value)
                    throw new ArgumentException("CAPACITY_EXCEEDED");
            }

            // Atomic hold + insert via transaction
            using (var tx = _db.Database.BeginTransaction())
            {
                // 1) Hold slot
                slot.Status = "Held";
                _slotRepo.Update(slot);

                // 2) Create appointment
                var appt = new Appointment
                {
                    PatientId = dto.PatientId,
                    ProviderId = slot.ProviderId,
                    SiteId = slot.SiteId,
                    ServiceId = slot.ServiceId,
                    SlotDate = slot.SlotDate,
                    StartTime = slot.StartTime,
                    EndTime = slot.EndTime,
                    Status = "Booked",
                    BookingChannel = dto.BookingChannel
                };
                _apptRepo.Add(appt);

                // 3) Project calendar event
                _calendarRepo.Add(new CalendarEvent
                {
                    EntityType = "Appointment",
                    EntityId = 0, // set after SaveChanges
                    ProviderId = appt.ProviderId,
                    SiteId = appt.SiteId,
                    RoomId = null,
                    StartTime = CombineUtc(appt.SlotDate, appt.StartTime),
                    EndTime = CombineUtc(appt.SlotDate, appt.EndTime),
                    Status = "Active"
                });

                // Persist to get AppointmentId
                _uow.SaveChanges();

                // Update event with actual AppointmentId
                _calendarRepo.SetLatestEntityId("Appointment", appt.AppointmentId);

                // 4) Notification + Reminder
                _notifRepo.Add(new Notification
                {
                    UserId = appt.PatientId,
                    Message = $"Your appointment is booked on {appt.SlotDate:yyyy-MM-dd} at {appt.StartTime:HH\\:mm}.",
                    Category = "Appointment",
                    Status = "Unread",
                    CreatedDate = DateTime.UtcNow
                });

                var defaultOffset = _configRepo.GetInt("reminder.default.offset.min", 1440) ?? 1440; // 24h
                _reminderRepo.Add(new ReminderSchedule
                {
                    AppointmentId = appt.AppointmentId,
                    RemindOffsetMin = defaultOffset,
                    Channel = "InApp",
                    Status = "Pending"
                });

                // 5) Audit
                _auditService.CreateAudit(new AuditLogCreateDto
                {
                    Action = "BookAppointment",
                    Resource = "Appointment",
                    Metadata = $"{{\"appointmentId\":{appt.AppointmentId},\"slotId\":{slot.PubSlotId}}}"
                });

                _uow.SaveChanges();
                tx.Commit();

                return Map(appt);
            }
        }

        public AppointmentResponseDto Reschedule(int appointmentId, RescheduleAppointmentRequestDto dto)
        {
            if (appointmentId <= 0) throw new ArgumentException("Invalid AppointmentId.");
            if (dto.NewPublishedSlotId <= 0) throw new ArgumentException("Invalid NewPublishedSlotId.");

            var appt = _apptRepo.GetById(appointmentId);
            if (appt == null) throw new KeyNotFoundException("Appointment not found.");
            if (appt.Status is "Completed" or "Cancelled" or "NoShow")
                throw new ArgumentException("INVALID_TRANSITION");

            var newSlot = _slotRepo.GetById(dto.NewPublishedSlotId);
            if (newSlot == null) throw new KeyNotFoundException("New slot not found.");
            if (!string.Equals(newSlot.Status, "Open", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("SLOT_UNAVAILABLE");

            // Old exact slot (if exists) to free later (Held -> Open). Closed never re-opens.
            var oldSlot = _slotRepo.FindExact(appt.ProviderId, appt.SiteId, appt.SlotDate, appt.StartTime, appt.EndTime);

            using (var tx = _db.Database.BeginTransaction())
            {
                // 1) Hold new slot
                newSlot.Status = "Held";
                _slotRepo.Update(newSlot);

                // 2) Update appointment time window
                var oldValues = new
                {
                    appt.SlotDate,
                    Start = appt.StartTime.ToString("HH:mm"),
                    End = appt.EndTime.ToString("HH:mm")
                };

                appt.SlotDate = newSlot.SlotDate;
                appt.StartTime = newSlot.StartTime;
                appt.EndTime = newSlot.EndTime;
                appt.ProviderId = newSlot.ProviderId;
                appt.SiteId = newSlot.SiteId;
                appt.ServiceId = newSlot.ServiceId;
                // Status remains Booked
                _apptRepo.Update(appt);

                // 3) Free old slot only if it was Held
                if (oldSlot != null && string.Equals(oldSlot.Status, "Held", StringComparison.OrdinalIgnoreCase))
                {
                    oldSlot.Status = "Open";
                    _slotRepo.Update(oldSlot);
                }

                // 4) AppointmentChange
                _changeRepo.Add(new AppointmentChange
                {
                    AppointmentId = appt.AppointmentId,
                    ChangeType = "Reschedule",
                    OldValuesJson = System.Text.Json.JsonSerializer.Serialize(oldValues),
                    NewValuesJson = System.Text.Json.JsonSerializer.Serialize(new
                    {
                        appt.SlotDate,
                        Start = appt.StartTime.ToString("HH:mm"),
                        End = appt.EndTime.ToString("HH:mm")
                    }),
                    ChangedBy = null,
                    ChangedDate = DateTime.UtcNow,
                    Reason = dto.Reason
                });

                // 5) Notification
                _notifRepo.Add(new Notification
                {
                    UserId = appt.PatientId,
                    Message = $"Your appointment was rescheduled to {appt.SlotDate:yyyy-MM-dd} at {appt.StartTime:HH\\:mm}.",
                    Category = "Change",
                    Status = "Unread",
                    CreatedDate = DateTime.UtcNow
                });

                // 6) Audit
                _auditService.CreateAudit(new AuditLogCreateDto
                {
                    Action = "RescheduleAppointment",
                    Resource = "Appointment",
                    Metadata = $"{{\"appointmentId\":{appt.AppointmentId},\"newSlotId\":{newSlot.PubSlotId}}}"
                });

                _uow.SaveChanges();
                tx.Commit();

                return Map(appt);
            }
        }

        public void Cancel(int appointmentId, CancelAppointmentRequestDto dto)
        {
            if (appointmentId <= 0) throw new ArgumentException("Invalid AppointmentId.");

            var appt = _apptRepo.GetById(appointmentId);
            if (appt == null) throw new KeyNotFoundException("Appointment not found.");
            if (appt.Status is "Completed" or "Cancelled" or "NoShow")
                throw new ArgumentException("INVALID_TRANSITION");

            // Find the exact slot to free (Held->Open). If Closed, we do NOT reopen.
            var slot = _slotRepo.FindExact(appt.ProviderId, appt.SiteId, appt.SlotDate, appt.StartTime, appt.EndTime);

            using (var tx = _db.Database.BeginTransaction())
            {
                // 1) Update appointment status
                appt.Status = "Cancelled";
                _apptRepo.Update(appt);

                // 2) Free slot if currently Held
                if (slot != null && string.Equals(slot.Status, "Held", StringComparison.OrdinalIgnoreCase))
                {
                    slot.Status = "Open";
                    _slotRepo.Update(slot);
                }

                // 3) AppointmentChange
                _changeRepo.Add(new AppointmentChange
                {
                    AppointmentId = appt.AppointmentId,
                    ChangeType = "Cancel",
                    OldValuesJson = System.Text.Json.JsonSerializer.Serialize(new { appt.SlotDate, Start = appt.StartTime.ToString("HH:mm"), End = appt.EndTime.ToString("HH:mm") }),
                    NewValuesJson = null,
                    ChangedBy = null,
                    ChangedDate = DateTime.UtcNow,
                    Reason = dto.Reason
                });

                // 4) Notification
                _notifRepo.Add(new Notification
                {
                    UserId = appt.PatientId,
                    Message = $"Your appointment on {appt.SlotDate:yyyy-MM-dd} at {appt.StartTime:HH\\:mm} has been cancelled.",
                    Category = "Change",
                    Status = "Unread",
                    CreatedDate = DateTime.UtcNow
                });

                // 5) Audit
                _auditService.CreateAudit(new AuditLogCreateDto
                {
                    Action = "CancelAppointment",
                    Resource = "Appointment",
                    Metadata = $"{{\"appointmentId\":{appt.AppointmentId}}}"
                });

                _uow.SaveChanges();
                tx.Commit();
            }
        }

        public AppointmentResponseDto GetById(int appointmentId)
        {
            var a = _apptRepo.GetById(appointmentId);
            if (a == null) throw new KeyNotFoundException("Appointment not found.");
            return Map(a);
        }

        public IEnumerable<AppointmentResponseDto> Search(AppointmentSearchRequestDto dto)
        {
            DateOnly? d = null;
            if (!string.IsNullOrWhiteSpace(dto.Date))
            {
                if (!DateOnly.TryParseExact(dto.Date.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
                    throw new ArgumentException("Invalid date format. Use yyyy-MM-dd.");
                d = parsed;
            }

            var items = _apptRepo.Search(dto.PatientId, dto.ProviderId, dto.SiteId, d, dto.Status);
            return items.Select(Map).ToList();
        }

        // ------------------- helpers -------------------
        private static DateTime CombineUtc(DateOnly date, TimeOnly time)
        {
            return new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, 0, DateTimeKind.Utc);
        }

        public void MarkCheckedIn(int appointmentId)
        {
            var appt = _apptRepo.GetById(appointmentId);
            if (appt == null) throw new KeyNotFoundException("Appointment not found.");
            if (appt.Status != "Booked") throw new ArgumentException("INVALID_TRANSITION");

            appt.Status = "CheckedIn";
            _apptRepo.Update(appt);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "MarkCheckedIn",
                Resource = "Appointment",
                Metadata = $"{{\"appointmentId\":{appointmentId}}}"
            });
        }

        public void MarkComplete(int appointmentId)
        {
            var appt = _apptRepo.GetById(appointmentId);
            if (appt == null) throw new KeyNotFoundException("Appointment not found.");
            if (appt.Status != "CheckedIn") throw new ArgumentException("INVALID_TRANSITION");

            appt.Status = "Completed";
            _apptRepo.Update(appt);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "MarkComplete",
                Resource = "Appointment",
                Metadata = $"{{\"appointmentId\":{appointmentId}}}"
            });
        }

        public void MarkNoShow(int appointmentId)
        {
            var appt = _apptRepo.GetById(appointmentId);
            if (appt == null) throw new KeyNotFoundException("Appointment not found.");
            if (appt.Status is "Completed" or "Cancelled" or "NoShow")
                throw new ArgumentException("INVALID_TRANSITION");

            appt.Status = "NoShow";
            _apptRepo.Update(appt);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "MarkNoShow",
                Resource = "Appointment",
                Metadata = $"{{\"appointmentId\":{appointmentId}}}"
            });
        }

        public void CancelByProvider(int appointmentId)
        {
            var appt = _apptRepo.GetById(appointmentId);
            if (appt == null) throw new KeyNotFoundException("Appointment not found.");
            if (appt.Status is "Completed" or "Cancelled" or "NoShow")
                throw new ArgumentException("INVALID_TRANSITION");

            appt.Status = "Cancelled";
            _apptRepo.Update(appt);

            _notifRepo.Add(new Notification
            {
                UserId = appt.PatientId,
                Message = $"Your appointment on {appt.SlotDate:yyyy-MM-dd} has been cancelled by the provider.",
                Category = "Change",
                Status = "Unread",
                CreatedDate = DateTime.UtcNow
            });

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action = "CancelByProvider",
                Resource = "Appointment",
                Metadata = $"{{\"appointmentId\":{appointmentId}}}"
            });
            _uow.SaveChanges();
        }

        private static AppointmentResponseDto Map(Appointment a)
        {
            return new AppointmentResponseDto
            {
                AppointmentId = a.AppointmentId,
                PatientId = a.PatientId,
                ProviderId = a.ProviderId,
                SiteId = a.SiteId,
                ServiceId = a.ServiceId,
                SlotDate = a.SlotDate,
                StartTime = a.StartTime.ToString("HH:mm"),
                EndTime = a.EndTime.ToString("HH:mm"),
                Status = a.Status,
                BookingChannel = a.BookingChannel
            };
        }
    }
}