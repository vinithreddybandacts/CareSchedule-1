using System;
using System.Collections.Generic;
using System.Linq;
using CareSchedule.DTOs;
using CareSchedule.Infrastructure;
using CareSchedule.Models;
using CareSchedule.Repositories.Interface;
using CareSchedule.Services.Interface;

namespace CareSchedule.Services.Implementation
{
    public class RosterService : IRosterService
    {
        private readonly IShiftTemplateRepository _shiftRepo;
        private readonly IRosterRepository _rosterRepo;
        private readonly IRosterAssignmentRepository _assignRepo;
        private readonly IOnCallCoverageRepository _onCallRepo;
        private readonly INotificationRepository _notifRepo;
        private readonly IAuditLogRepository _auditRepo;
        private readonly IUnitOfWork _uow;

        public RosterService(
            IShiftTemplateRepository shiftRepo,
            IRosterRepository rosterRepo,
            IRosterAssignmentRepository assignRepo,
            IOnCallCoverageRepository onCallRepo,
            INotificationRepository notifRepo,
            IAuditLogRepository auditRepo,
            IUnitOfWork uow)
        {
            _shiftRepo = shiftRepo;
            _rosterRepo = rosterRepo;
            _assignRepo = assignRepo;
            _onCallRepo = onCallRepo;
            _notifRepo = notifRepo;
            _auditRepo = auditRepo;
            _uow = uow;
        }

        // --------- Shift Templates ---------

        public ShiftTemplateResponseDto CreateShiftTemplate(CreateShiftTemplateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name)) throw new ArgumentException("Name is required.");
            if (string.IsNullOrWhiteSpace(dto.Role)) throw new ArgumentException("Role is required.");
            if (dto.SiteId <= 0) throw new ArgumentException("SiteId is required.");

            var startTime = TimeOnly.Parse(dto.StartTime);
            var endTime = TimeOnly.Parse(dto.EndTime);

            var e = new ShiftTemplate
            {
                Name = dto.Name.Trim(),
                StartTime = startTime,
                EndTime = endTime,
                BreakMinutes = dto.BreakMinutes,
                Role = dto.Role.Trim(),
                SiteId = dto.SiteId,
                Status = "Active"
            };

            _shiftRepo.Add(e);

            _auditRepo.Create(new AuditLog
            {
                Action = "CREATE",
                Resource = "ShiftTemplate",
                Timestamp = DateTime.UtcNow,
                Metadata = "{\"id\":" + e.ShiftTemplateId + "}"
            });

            return MapShiftTemplate(e);
        }

        public ShiftTemplateResponseDto UpdateShiftTemplate(int id, UpdateShiftTemplateDto dto)
        {
            var e = _shiftRepo.GetById(id);
            if (e is null) throw new KeyNotFoundException("ShiftTemplate not found.");

            if (!string.IsNullOrWhiteSpace(dto.Name)) e.Name = dto.Name.Trim();
            if (!string.IsNullOrWhiteSpace(dto.StartTime)) e.StartTime = TimeOnly.Parse(dto.StartTime);
            if (!string.IsNullOrWhiteSpace(dto.EndTime)) e.EndTime = TimeOnly.Parse(dto.EndTime);
            if (dto.BreakMinutes.HasValue) e.BreakMinutes = dto.BreakMinutes.Value;
            if (!string.IsNullOrWhiteSpace(dto.Role)) e.Role = dto.Role.Trim();
            if (!string.IsNullOrWhiteSpace(dto.Status)) e.Status = dto.Status.Trim();

            _shiftRepo.Update(e);

            _auditRepo.Create(new AuditLog
            {
                Action = "UPDATE",
                Resource = "ShiftTemplate",
                Timestamp = DateTime.UtcNow,
                Metadata = "{\"id\":" + e.ShiftTemplateId + "}"
            });

            return MapShiftTemplate(e);
        }

        public void DeleteShiftTemplate(int id)
        {
            var e = _shiftRepo.GetById(id);
            if (e is null) throw new KeyNotFoundException("ShiftTemplate not found.");

            e.Status = "Inactive";
            _shiftRepo.Update(e);

            _auditRepo.Create(new AuditLog
            {
                Action = "DELETE",
                Resource = "ShiftTemplate",
                Timestamp = DateTime.UtcNow,
                Metadata = "{\"id\":" + e.ShiftTemplateId + "}"
            });
        }

        // --------- Rosters ---------

        public RosterResponseDto CreateRoster(CreateRosterDto dto)
        {
            if (dto.SiteId <= 0) throw new ArgumentException("SiteId is required.");

            var periodStart = DateOnly.Parse(dto.PeriodStart);
            var periodEnd = DateOnly.Parse(dto.PeriodEnd);

            if (periodEnd <= periodStart) throw new ArgumentException("PeriodEnd must be after PeriodStart.");

            var e = new Roster
            {
                SiteId = dto.SiteId,
                Department = dto.Department,
                PeriodStart = periodStart,
                PeriodEnd = periodEnd,
                Status = "Draft"
            };

            _rosterRepo.Add(e);

            _auditRepo.Create(new AuditLog
            {
                Action = "CREATE",
                Resource = "Roster",
                Timestamp = DateTime.UtcNow,
                Metadata = "{\"id\":" + e.RosterId + "}"
            });

            return MapRoster(e);
        }

        public RosterResponseDto PublishRoster(int rosterId, PublishRosterDto dto)
        {
            var e = _rosterRepo.GetById(rosterId);
            if (e is null) throw new KeyNotFoundException("Roster not found.");

            if (e.Status != "Draft") throw new ArgumentException("Only draft rosters can be published.");

            e.Status = "Published";
            e.PublishedBy = dto.PublishedBy;
            e.PublishedDate = DateTime.UtcNow;

            _rosterRepo.Update(e);

            var assignments = _assignRepo.Search(e.SiteId, null, null, null);
            var distinctUserIds = assignments.Select(a => a.UserId).Distinct();

            foreach (var userId in distinctUserIds)
            {
                var notif = new Notification
                {
                    UserId = userId,
                    Message = "Roster published for period " + e.PeriodStart.ToString("yyyy-MM-dd") + " to " + e.PeriodEnd.ToString("yyyy-MM-dd"),
                    Category = "Roster",
                    Status = "Unread",
                    CreatedDate = DateTime.UtcNow
                };
                _notifRepo.Add(notif);
            }

            _auditRepo.Create(new AuditLog
            {
                Action = "UPDATE",
                Resource = "Roster",
                Timestamp = DateTime.UtcNow,
                Metadata = "{\"id\":" + e.RosterId + "}"
            });

            return MapRoster(e);
        }

        // --------- Assignments ---------

        public RosterAssignmentResponseDto AssignStaff(CreateRosterAssignmentDto dto)
        {
            if (dto.RosterId <= 0) throw new ArgumentException("RosterId is required.");
            if (dto.UserId <= 0) throw new ArgumentException("UserId is required.");
            if (dto.ShiftTemplateId <= 0) throw new ArgumentException("ShiftTemplateId is required.");

            var roster = _rosterRepo.GetById(dto.RosterId);
            if (roster is null) throw new KeyNotFoundException("Roster not found.");

            if (roster.Status != "Draft") throw new ArgumentException("Can only assign to draft rosters.");

            var shift = _shiftRepo.GetById(dto.ShiftTemplateId);
            if (shift is null) throw new KeyNotFoundException("ShiftTemplate not found.");

            var date = DateOnly.Parse(dto.Date);

            var existing = _assignRepo.Search(null, dto.UserId, date, null)
                .FirstOrDefault(a => a.RosterId == dto.RosterId && a.ShiftTemplateId == dto.ShiftTemplateId);
            if (existing is not null) throw new ArgumentException("Assignment already exists for this user, date, and shift.");

            var e = new RosterAssignment
            {
                RosterId = dto.RosterId,
                UserId = dto.UserId,
                ShiftTemplateId = dto.ShiftTemplateId,
                Date = date,
                Role = dto.Role,
                Status = "Assigned"
            };

            _assignRepo.Add(e);

            _auditRepo.Create(new AuditLog
            {
                Action = "CREATE",
                Resource = "RosterAssignment",
                Timestamp = DateTime.UtcNow,
                Metadata = "{\"id\":" + e.AssignmentId + "}"
            });

            return MapRosterAssignment(e);
        }

        public RosterAssignmentResponseDto SwapShift(int assignmentId, SwapAssignmentDto dto)
        {
            var e = _assignRepo.GetById(assignmentId);
            if (e is null) throw new KeyNotFoundException("RosterAssignment not found.");

            if (e.Status != "Assigned") throw new ArgumentException("Only assigned shifts can be swapped.");

            if (dto.NewUserId.HasValue && dto.NewUserId.Value > 0)
            {
                e.UserId = dto.NewUserId.Value;
            }

            if (dto.NewShiftTemplateId.HasValue && dto.NewShiftTemplateId.Value > 0)
            {
                var shift = _shiftRepo.GetById(dto.NewShiftTemplateId.Value);
                if (shift is null) throw new KeyNotFoundException("ShiftTemplate not found.");
                e.ShiftTemplateId = dto.NewShiftTemplateId.Value;
            }

            e.Status = "Swapped";

            _assignRepo.Update(e);

            _notifRepo.Add(new Notification
            {
                UserId = e.UserId,
                Message = "Your shift has been swapped for " + e.Date.ToString("yyyy-MM-dd"),
                Category = "Roster",
                Status = "Unread",
                CreatedDate = DateTime.UtcNow
            });

            _auditRepo.Create(new AuditLog
            {
                Action = "UPDATE",
                Resource = "RosterAssignment",
                Timestamp = DateTime.UtcNow,
                Metadata = "{\"id\":" + e.AssignmentId + "}"
            });

            return MapRosterAssignment(e);
        }

        public void MarkAbsent(int assignmentId)
        {
            var e = _assignRepo.GetById(assignmentId);
            if (e is null) throw new KeyNotFoundException("RosterAssignment not found.");

            e.Status = "Absent";
            _assignRepo.Update(e);

            _auditRepo.Create(new AuditLog
            {
                Action = "UPDATE",
                Resource = "RosterAssignment",
                Timestamp = DateTime.UtcNow,
                Metadata = "{\"id\":" + e.AssignmentId + "}"
            });
        }

        public IEnumerable<RosterAssignmentResponseDto> SearchAssignments(RosterAssignmentSearchDto dto)
        {
            DateOnly? date = null;
            if (!string.IsNullOrWhiteSpace(dto.Date))
            {
                date = DateOnly.Parse(dto.Date);
            }

            var results = _assignRepo.Search(dto.SiteId, dto.UserId, date, dto.Status);
            var list = new List<RosterAssignmentResponseDto>(results.Count());
            foreach (var a in results)
            {
                list.Add(MapRosterAssignment(a));
            }
            return list;
        }

        // --------- On-Call ---------

        public OnCallResponseDto CreateOnCall(CreateOnCallDto dto)
        {
            if (dto.SiteId <= 0) throw new ArgumentException("SiteId is required.");
            if (dto.PrimaryUserId <= 0) throw new ArgumentException("PrimaryUserId is required.");

            var date = DateOnly.Parse(dto.Date);
            var startTime = TimeOnly.Parse(dto.StartTime);
            var endTime = TimeOnly.Parse(dto.EndTime);

            if (endTime <= startTime) throw new ArgumentException("EndTime must be after StartTime.");

            if (dto.BackupUserId.HasValue && dto.BackupUserId.Value == dto.PrimaryUserId)
            {
                throw new ArgumentException("BackupUserId cannot be the same as PrimaryUserId.");
            }

            var e = new OnCallCoverage
            {
                SiteId = dto.SiteId,
                Department = dto.Department,
                Date = date,
                StartTime = startTime,
                EndTime = endTime,
                PrimaryUserId = dto.PrimaryUserId,
                BackupUserId = dto.BackupUserId,
                Status = "Active"
            };

            _onCallRepo.Add(e);

            _auditRepo.Create(new AuditLog
            {
                Action = "CREATE",
                Resource = "OnCallCoverage",
                Timestamp = DateTime.UtcNow,
                Metadata = "{\"id\":" + e.OnCallId + "}"
            });

            return MapOnCall(e);
        }

        public OnCallResponseDto UpdateOnCall(int id, UpdateOnCallDto dto)
        {
            var e = _onCallRepo.GetById(id);
            if (e is null) throw new KeyNotFoundException("OnCallCoverage not found.");

            if (!string.IsNullOrWhiteSpace(dto.Department)) e.Department = dto.Department;
            if (!string.IsNullOrWhiteSpace(dto.Date)) e.Date = DateOnly.Parse(dto.Date);
            if (!string.IsNullOrWhiteSpace(dto.StartTime)) e.StartTime = TimeOnly.Parse(dto.StartTime);
            if (!string.IsNullOrWhiteSpace(dto.EndTime)) e.EndTime = TimeOnly.Parse(dto.EndTime);
            if (dto.PrimaryUserId.HasValue && dto.PrimaryUserId.Value > 0) e.PrimaryUserId = dto.PrimaryUserId.Value;
            if (dto.BackupUserId.HasValue) e.BackupUserId = dto.BackupUserId.Value;
            if (!string.IsNullOrWhiteSpace(dto.Status)) e.Status = dto.Status.Trim();

            _onCallRepo.Update(e);

            _auditRepo.Create(new AuditLog
            {
                Action = "UPDATE",
                Resource = "OnCallCoverage",
                Timestamp = DateTime.UtcNow,
                Metadata = "{\"id\":" + e.OnCallId + "}"
            });

            return MapOnCall(e);
        }

        private static ShiftTemplateResponseDto MapShiftTemplate(ShiftTemplate e) => new()
        {
            ShiftTemplateId = e.ShiftTemplateId,
            Name = e.Name,
            StartTime = e.StartTime.ToString("HH:mm"),
            EndTime = e.EndTime.ToString("HH:mm"),
            BreakMinutes = e.BreakMinutes,
            Role = e.Role,
            SiteId = e.SiteId,
            Status = e.Status
        };

        private static RosterResponseDto MapRoster(Roster e) => new()
        {
            RosterId = e.RosterId,
            SiteId = e.SiteId,
            Department = e.Department,
            PeriodStart = e.PeriodStart.ToString("yyyy-MM-dd"),
            PeriodEnd = e.PeriodEnd.ToString("yyyy-MM-dd"),
            PublishedBy = e.PublishedBy,
            PublishedDate = e.PublishedDate,
            Status = e.Status
        };

        private static RosterAssignmentResponseDto MapRosterAssignment(RosterAssignment e) => new()
        {
            AssignmentId = e.AssignmentId,
            RosterId = e.RosterId,
            UserId = e.UserId,
            ShiftTemplateId = e.ShiftTemplateId,
            Date = e.Date.ToString("yyyy-MM-dd"),
            Role = e.Role,
            Status = e.Status
        };

        private static OnCallResponseDto MapOnCall(OnCallCoverage e) => new()
        {
            OnCallId = e.OnCallId,
            SiteId = e.SiteId,
            Department = e.Department,
            Date = e.Date.ToString("yyyy-MM-dd"),
            StartTime = e.StartTime.ToString("HH:mm"),
            EndTime = e.EndTime.ToString("HH:mm"),
            PrimaryUserId = e.PrimaryUserId,
            BackupUserId = e.BackupUserId,
            Status = e.Status
        };
    }
}
