using System;
using System.Globalization;
using System.Text.Json;
using CareSchedule.Models;
using CareSchedule.Services.Interface;
using CareSchedule.DTOs;
using CareSchedule.Repositories.Interface;
using CareSchedule.Infrastructure;

namespace CareSchedule.Services.Implementation
{
    /// <summary>
    /// Availability module service (synchronous). Owns:
    /// - AvailabilityTemplate
    /// - AvailabilityBlock
    /// - PublishedSlot (generation & maintenance; NO public write API)
    /// - CalendarEvent projection for Blocks
    /// 
    /// Rules enforced:
    /// - Append-only AuditLog on mutations
    /// - PublishedSlot created ONLY by Availability
    /// - Closed slots never reopen
    /// - Controllers remain thin; all logic here
    /// </summary>
    public class AvailabilityService(
            IAvailabilityTemplateRepository _templateRepo,
            IAvailabilityBlockRepository _blockRepo,
            IPublishedSlotRepository _slotRepo,
            ICalendarEventRepository _calendarRepo,
            IAuditLogService _auditService,
            ISiteRepository _siteRepo,
            IProviderRepository _providerRepo,
            IServiceRepository _serviceRepo,
            IProviderServiceRepository _providerServiceRepo,
            IUnitOfWork _uow)
            : IAvailabilityService
    {
        public void EnsureSiteActive(int siteId)
        {
            if (siteId <= 0) throw new ArgumentException("Invalid SiteID.");

            var site = _siteRepo.Get(siteId);
            if (site == null) throw new KeyNotFoundException("Site not found.");
            if (!string.Equals(site.Status, "Active", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Site is not active.");
        }

        public void EnsureProviderActive(int providerId)
        {
            if (providerId <= 0) throw new ArgumentException("Invalid ProviderID.");

            var provider = _providerRepo.GetById(providerId);
            if (provider == null) throw new KeyNotFoundException("Provider not found.");
            if (!string.Equals(provider.Status, "Active", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Provider is not active.");
        }

        public void EnsureServiceActive(int serviceId)
        {
            if (serviceId <= 0) throw new ArgumentException("Invalid ServiceID.");

            var service = _serviceRepo.GetById(serviceId);
            if (service == null) throw new KeyNotFoundException("Service not found.");
            if (!string.Equals(service.Status, "Active", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Service is not active.");
        }

        public void EnsureProviderOffersServiceActive(int providerId, int serviceId)
        {
            if (providerId <= 0) throw new ArgumentException("Invalid ProviderID.");
            if (serviceId  <= 0) throw new ArgumentException("Invalid ServiceID.");

            var ps = _providerServiceRepo.GetByProviderAndService(providerId, serviceId);
            if (ps == null) throw new KeyNotFoundException("Provider does not offer this service.");
            if (!string.Equals(ps.Status, "Active", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("ProviderService mapping is not active.");
        }

        // =========================================================
        // Templates
        // =========================================================
        public int CreateTemplate(CreateAvailabilityTemplateRequestDto dto)
        {
            
            EnsureProviderActive(dto.ProviderId);
            EnsureSiteActive(dto.SiteId);
            ValidateTemplateDto(dto);

            var entity = new AvailabilityTemplate
            {
                ProviderId      = dto.ProviderId,
                SiteId          = dto.SiteId,
                DayOfWeek       = (byte)dto.DayOfWeek,
                StartTime       = ParseTimeOnly(dto.StartTime),
                EndTime         = ParseTimeOnly(dto.EndTime),
                SlotDurationMin = dto.SlotDurationMin,
                Status          = dto.Status?.Trim() ?? "Active"
            };

            _templateRepo.Add(entity);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action    = "CreateAvailabilityTemplate",
                Resource  = "AvailabilityTemplate",
                Metadata  = SerializeJson(new
                {
                    entity.ProviderId,
                    entity.SiteId,
                    entity.DayOfWeek,
                    Start = entity.StartTime.ToString(@"hh\:mm"),
                    End   = entity.EndTime.ToString(@"hh\:mm"),
                    entity.SlotDurationMin,
                    entity.Status
                })
            });

            _uow.SaveChanges();
            return entity.TemplateId; // populated by EF on SaveChanges
        }

        public void UpdateTemplate(UpdateAvailabilityTemplateRequestDto dto)
        {
            if (dto.TemplateId <= 0) throw new ArgumentException("Invalid TemplateID.");

            EnsureProviderActive(dto.ProviderId);
            EnsureSiteActive(dto.SiteId);

            ValidateTemplateDto(dto);

            var entity = _templateRepo.GetById(dto.TemplateId);
            if (entity == null) throw new KeyNotFoundException("Template not found.");

            var before = new
            {
                entity.ProviderId,
                entity.SiteId,
                entity.DayOfWeek,
                Start = entity.StartTime.ToString(@"hh\:mm"),
                End   = entity.EndTime.ToString(@"hh\:mm"),
                entity.SlotDurationMin,
                entity.Status
            };

            entity.ProviderId      = dto.ProviderId;
            entity.SiteId          = dto.SiteId;
            entity.DayOfWeek       = (byte)dto.DayOfWeek;
            entity.StartTime       = ParseTimeOnly(dto.StartTime);
            entity.EndTime         = ParseTimeOnly(dto.EndTime);
            entity.SlotDurationMin = dto.SlotDurationMin;
            entity.Status          = dto.Status?.Trim() ?? entity.Status;

            _templateRepo.Update(entity);

            var after = new
            {
                entity.ProviderId,
                entity.SiteId,
                entity.DayOfWeek,
                Start = entity.StartTime.ToString(@"hh\:mm"),
                End   = entity.EndTime.ToString(@"hh\:mm"),
                entity.SlotDurationMin,
                entity.Status
            };

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action    = "UpdateAvailabilityTemplate",
                Resource  = "AvailabilityTemplate",
                Metadata  = SerializeJson(new
                {
                    entity.TemplateId,
                    before,
                    after
                })
            });

            _uow.SaveChanges();
        }

        public IEnumerable<AvailabilityTemplateResponseDto> ListTemplates(int providerId, int siteId)
        {
            EnsureProviderActive(providerId);
            EnsureSiteActive(siteId);

            var items = _templateRepo.List(providerId, siteId);
            return items.Select(t => new AvailabilityTemplateResponseDto
            {
                TemplateId      = t.TemplateId,
                ProviderId      = t.ProviderId,
                SiteId          = t.SiteId,
                DayOfWeek       = t.DayOfWeek,
                StartTime       = t.StartTime.ToString(@"hh\:mm"),
                EndTime         = t.EndTime.ToString(@"hh\:mm"),
                SlotDurationMin = t.SlotDurationMin,
                Status          = t.Status
            }).ToList();
        }

        // =========================================================
        // Blocks
        // =========================================================
        public int CreateBlock(CreateAvailabilityBlockRequestDto dto)
        {
            // ---------- Cross-module validations ----------
            EnsureProviderActive(dto.ProviderId);
            EnsureSiteActive(dto.SiteId);

            ValidateBlockDto(dto);

            var date  = ParseDateOnly(dto.Date);     // DateOnly
            var start = ParseTimeOnly(dto.StartTime); // TimeOnly
            var end   = ParseTimeOnly(dto.EndTime);   // TimeOnly
            if (end <= start) throw new ArgumentException("EndTime must be after StartTime.");

            // ---------- 1) Strict overlap rejection (no merge) ----------
            var sameDayActiveBlocks = _blockRepo
                .List(dto.ProviderId, dto.SiteId, date)
                .Where(b => string.Equals(b.Status, "Active", StringComparison.OrdinalIgnoreCase))
                .ToList();

            // Overlap if (b.Start < end) && (start < b.End)  (end-exclusive)
            var overlap = sameDayActiveBlocks.FirstOrDefault(b => b.StartTime < end && start < b.EndTime);
            if (overlap != null)
            {
                // Reject creation — let middleware map to 400 BAD_REQUEST
                var msg = $"Overlaps with existing block (ID={overlap.BlockId}) window {overlap.StartTime:HH\\:mm}-{overlap.EndTime:HH\\:mm} on {date:yyyy-MM-dd}.";
                throw new ArgumentException(msg);
            }

            // ---------- 2) Create block row ----------
            var block = new AvailabilityBlock
            {
                ProviderId = dto.ProviderId,
                SiteId     = dto.SiteId,
                Date       = date,
                StartTime  = start,
                EndTime    = end,
                Reason     = dto.Reason?.Trim() ?? string.Empty,
                Status     = dto.Status?.Trim() ?? "Active"
            };

            _blockRepo.Add(block);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action    = "CreateAvailabilityBlock",
                Resource  = "AvailabilityBlock",
                Metadata  = SerializeJson(new
                {
                    dto.ProviderId,
                    dto.SiteId,
                    dto.Date,
                    dto.StartTime,
                    dto.EndTime,
                    Reason = block.Reason
                })
            });

            // First commit to obtain BlockID for CalendarEvent projection
            _uow.SaveChanges();

            // ---------- 3) Close overlapping Open/Held slots (end-exclusive) ----------
            var overlappingSlots = _slotRepo.FindSlotsInWindow(
                block.ProviderId, block.SiteId, block.Date, start, end, "Open", "Held");

            foreach (var s in overlappingSlots)
            {
                s.Status = "Closed";   // Closed never re-opens
                _slotRepo.Update(s);
            }

            // ---------- 4) Project to CalendarEvent ----------
            var startDt = CombineUtc(block.Date, start); // UTC DateTime
            var endDt   = CombineUtc(block.Date, end);

            _calendarRepo.Add(new CalendarEvent
            {
                EntityType = "Block",
                EntityId   = block.BlockId,
                ProviderId = block.ProviderId,
                SiteId     = block.SiteId,
                RoomId     = null,
                StartTime  = startDt,
                EndTime    = endDt,
                Status     = "Active"
            });

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action    = "CreateBlock_CloseSlots_ProjectCalendar",
                Resource  = "AvailabilityBlock",
                Metadata  = SerializeJson(new
                {
                    blockId    = block.BlockId,
                    providerId = block.ProviderId,
                    siteId     = block.SiteId,
                    date       = block.Date.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
                    start      = start.ToString("HH:mm"),
                    end        = end.ToString("HH:mm"),
                    closedSlots = overlappingSlots.Select(s => s.PubSlotId).ToList()
                })
            });

            _uow.SaveChanges();
            return block.BlockId;
        }

        public void RemoveBlock(int blockId)
        {
            if (blockId <= 0) throw new ArgumentException("Invalid BlockID.");

            var block = _blockRepo.GetById(blockId);
            if (block == null) throw new KeyNotFoundException("Block not found.");
         
            EnsureProviderActive(block.ProviderId);
            EnsureSiteActive(block.SiteId);

            var prev = block.Status;
            // Soft delete (no hard deletes): mark as Cancelled
            block.Status = "Cancelled";
            _blockRepo.Update(block);

            // Clean the projection
            _calendarRepo.DeleteByEntity("Block", blockId);

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action    = "RemoveAvailabilityBlock",
                Resource  = "AvailabilityBlock",
                Metadata  = SerializeJson(new { blockId, previousStatus = prev })
            });

            _uow.SaveChanges();
        }

        public IEnumerable<AvailabilityBlockResponseDto> ListBlocks(int providerId, int siteId, string? date)
        {
            EnsureProviderActive(providerId);
            EnsureSiteActive(siteId);

            if (providerId <= 0 || siteId <= 0)
                throw new ArgumentException("providerId and siteId are required.");

            DateOnly? d = null;
            if (!string.IsNullOrWhiteSpace(date))
                d = ParseDateOnly(date);

            var items = _blockRepo.List(providerId, siteId, d);
            return items.Select(b => new AvailabilityBlockResponseDto
            {
                BlockId   = b.BlockId,
                ProviderId= b.ProviderId,
                SiteId    = b.SiteId,
                Date      = b.Date.ToString("yyyy-MM-dd"),
                StartTime = b.StartTime.ToString(@"hh\:mm"),
                EndTime   = b.EndTime.ToString(@"hh\:mm"),
                Reason    = b.Reason ?? string.Empty,
                Status    = b.Status
            }).ToList();
        }

        // =========================================================
        // Slots (Read-only)
        // =========================================================

        public IEnumerable<SlotResponseDto> GetOpenSlots(SlotSearchRequestDto dto)
        {
            if (dto.ProviderId <= 0 || dto.ServiceId <= 0 || dto.SiteId <= 0)
                throw new ArgumentException("InvalId inputs.");
            if (string.IsNullOrWhiteSpace(dto.Date))
                throw new ArgumentException("Date is required.");

            EnsureProviderActive(dto.ProviderId);
            EnsureSiteActive(dto.SiteId);
            EnsureServiceActive(dto.ServiceId);
            EnsureProviderOffersServiceActive(dto.ProviderId, dto.ServiceId);

            var d     = ParseDateOnly(dto.Date);
            var slots = _slotRepo.GetOpenSlots(dto.ProviderId, dto.ServiceId, dto.SiteId, d);

            return slots.Select(s => new SlotResponseDto
            {
                PubSlotId  = s.PubSlotId,
                ProviderId = s.ProviderId,
                ServiceId  = s.ServiceId,
                SiteId     = s.SiteId,
                SlotDate   = s.SlotDate.ToString("yyyy-MM-dd"),
                StartTime  = s.StartTime.ToString(@"hh\:mm"),
                EndTime    = s.EndTime.ToString(@"hh\:mm"),
                Status     = s.Status
            }).ToList();
        }

        // =========================================================
        // Slot generation (MVP trigger)
        // =========================================================
        public GenerateSlotsResponseDto GenerateSlots(GenerateSlotsRequestDto dto)
        {
            EnsureSiteActive(dto.SiteId);
            if (dto.Days   <= 0) throw new ArgumentException("Days must be positive.");

            // MVP approach:
            // - Group templates by provider for the given site
            // - For each day in horizon, expand templates into slot intervals
            // - Insert only missing slots (idempotent behavior)
            var inserted = 0;
            var skipped  = 0;

            var templates = _templateRepo.ListBySiteActive(dto.SiteId).ToList();
            var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);

            for (int offset = 0; offset < dto.Days; offset++)
            {
                var day = today.AddDays(offset);
                var dow = (int)day.DayOfWeek;

                var todaysTemplates = templates.Where(t =>
                    t.DayOfWeek == (byte)dow &&
                    t.Status.Equals("Active", StringComparison.OrdinalIgnoreCase)).ToList();

                foreach (var t in todaysTemplates)
                {
                    // NEW: load active services for this provider
                    var services = _providerServiceRepo.GetActiveByProvider(t.ProviderId).ToList();
                    if (services.Count == 0)
                    {
                        // optional: audit skip reason
                        continue;
                    }

                    foreach (var ps in services)
                    {
                        var current = t.StartTime;
                        while (current < t.EndTime)
                        {
                            var next = current.AddMinutes(t.SlotDurationMin);
                            if (next > t.EndTime) break;

                            // Idempotency: also compare ServiceId now
                            var overlaps = _slotRepo.FindSlotsInWindow(t.ProviderId, t.SiteId, day, current, next,
                                "Open", "Held", "Closed");

                            var exact = overlaps.Any(s =>
                                s.SlotDate == day &&
                                s.StartTime == current &&
                                s.EndTime == next &&
                                s.ServiceId == ps.ServiceId); // <<< include ServiceId

                            if (!exact)
                            {
                                _slotRepo.AddRange(new[]
                                {
                                    new PublishedSlot
                                    {
                                        ProviderId = t.ProviderId,
                                        SiteId     = t.SiteId,
                                        ServiceId  = ps.ServiceId, // <<< set a valid ServiceId
                                        SlotDate   = day,
                                        StartTime  = current,
                                        EndTime    = next,
                                        Status     = "Open"
                                    }
                                });
                                inserted++;
                            }
                            else
                            {
                                skipped++;
                            }

                            current = next;
                        }
                    }
                }
            }

            _auditService.CreateAudit(new AuditLogCreateDto
            {
                Action    = "GenerateSlots",
                Resource  = "PublishedSlot",
                Metadata  = SerializeJson(new { dto.SiteId, dto.Days, inserted, skipped })
            });

            _uow.SaveChanges();

            return new GenerateSlotsResponseDto
            {
                InsertedCount         = inserted,
                SkippedExistingCount  = skipped
            };
        }

        // =========================================================
        // Helpers
        // =========================================================
        private static void ValidateTemplateDto(CreateAvailabilityTemplateRequestDto dto)
        {
            if (dto.ProviderId<= 0)            throw new ArgumentException("InvalId ProviderID.");
            if (dto.SiteId <= 0)            throw new ArgumentException("Invalid SiteID.");
            if (dto.DayOfWeek  < 0 || dto.DayOfWeek > 6)
                                                throw new ArgumentException("DayOfWeek must be 0-6.");
            if (string.IsNullOrWhiteSpace(dto.StartTime))
                                                throw new ArgumentException("StartTime is required.");
            if (string.IsNullOrWhiteSpace(dto.EndTime))
                                                throw new ArgumentException("EndTime is required.");
            if (dto.SlotDurationMin <= 0)       throw new ArgumentException("SlotDurationMin must be positive.");

            var start = ParseTimeOnly(dto.StartTime);
            var end   = ParseTimeOnly(dto.EndTime);
            if (end <= start)                    throw new ArgumentException("EndTime must be after StartTime.");
        }

        private static DateOnly ParseDateOnly(string yyyyMMdd)
        {
            if (!DateOnly.TryParseExact(yyyyMMdd.Trim(), "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var d))
                throw new ArgumentException("Invalid date format. Use yyyy-MM-dd.");
            return d;
        }

        private static void ValidateBlockDto(CreateAvailabilityBlockRequestDto dto)
        {
            if (dto.ProviderId <= 0)             throw new ArgumentException("Invalid ProviderID.");
            if (dto.SiteId    <= 0)             throw new ArgumentException("Invalid SiteID.");
            if (string.IsNullOrWhiteSpace(dto.Date))
                                                 throw new ArgumentException("Date is required.");
            if (string.IsNullOrWhiteSpace(dto.StartTime))
                                                 throw new ArgumentException("StartTime is required.");
            if (string.IsNullOrWhiteSpace(dto.EndTime))
                                                 throw new ArgumentException("EndTime is required.");
        }
                
        private static TimeOnly ParseTimeOnly(string hhmm)
        {
            if (!TimeOnly.TryParseExact(hhmm.Trim(), "HH:mm",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var t))
                throw new ArgumentException("Invalid time format. Use HH:mm.");
            return t;
        }

        private static DateTime CombineUtc(DateOnly date, TimeOnly time)
        {
            return new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, 0, DateTimeKind.Utc);
        }

        private static string SerializeJson(object obj)
        {
            // Lightweight JSON serialization for AuditLog.Metadata
            return JsonSerializer.Serialize(obj, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }

        /// <summary>
        /// MVP helper that groups templates by Provider for a given Site.
        /// NOTE: Replace with a repository method like ListBySiteActive(siteId) for production.
        /// </summary>
        private Dictionary<int, List<AvailabilityTemplate>> GroupTemplatesByProvider(int siteId)
        {
            var buckets = new Dictionary<int, List<AvailabilityTemplate>>();

            // Heuristic (MVP): probe a reasonable providerId range. Replace with a proper repo later.
            for (int providerId = 1; providerId <= 2000; providerId++)
            {
                var list = _templateRepo.List(providerId, siteId).ToList();
                if (list.Count == 0) continue;
                buckets[providerId] = list;
            }

            return buckets;
        }
    }
}