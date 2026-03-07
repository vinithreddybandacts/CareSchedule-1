using System.Text.Json;
using CareSchedule.DTOs;
using CareSchedule.Models;
using CareSchedule.Repositories.Interface;
using CareSchedule.Services.Interface;

namespace CareSchedule.Services.Implementation
{
    public class SiteService : ISiteService
    {
        private readonly ISiteRepository _repo;

        public SiteService(ISiteRepository repo)
        {
            _repo = repo;
        }

        public List<SiteDto> SearchSite(SiteSearchQuery query)
        {
            var (items, _) = _repo.SearchAsync(
                query.Name,
                query.Status,
                query.Page,
                query.PageSize,
                query.SortBy,
                query.SortDir,
                CancellationToken.None
            ).GetAwaiter().GetResult();

            return items.Select(Map).ToList();
        }

        public SiteDto? GetSite(int id)
        {
            var site = _repo.GetAsync(id, CancellationToken.None)
                            .GetAwaiter().GetResult();
            return site is null ? null : Map(site);
        }

        public SiteDto CreateSite(SiteCreateDto dto)
        {
            Validate(dto);

            var entity = new Site
            {
                Name = dto.Name.Trim(),
                AddressJson = NormalizeJson(dto.AddressJson),
                Timezone = dto.Timezone,
                Status = "Active"
            };

            entity = _repo.CreateAsync(entity, CancellationToken.None)
                          .GetAwaiter().GetResult();

            return Map(entity);
        }

        public SiteDto UpdateSite(int id, SiteUpdateDto dto)
        {
            var entity = _repo.GetAsync(id, CancellationToken.None)
                              .GetAwaiter().GetResult()
                        ?? throw new KeyNotFoundException("Site not found.");

            if (!string.IsNullOrWhiteSpace(dto.Name))
                entity.Name = dto.Name.Trim();

            if (dto.AddressJson is not null)
                entity.AddressJson = NormalizeJson(dto.AddressJson);

            if (!string.IsNullOrWhiteSpace(dto.Timezone))
            {
                ValidateTimezone(dto.Timezone);
                entity.Timezone = dto.Timezone;
            }

            _repo.UpdateAsync(entity, CancellationToken.None)
                 .GetAwaiter().GetResult();

            return Map(entity);
        }

        public void DeactivateSite(int id)
        {
            var entity = _repo.GetAsync(id, CancellationToken.None)
                              .GetAwaiter().GetResult()
                        ?? throw new KeyNotFoundException("Site not found.");

            if (entity.Status == "Inactive") return;

            entity.Status = "Inactive";

            _repo.UpdateAsync(entity, CancellationToken.None)
                 .GetAwaiter().GetResult();
        }

        public void ActivateSite(int id)
        {
            var entity = _repo.GetAsync(id, CancellationToken.None)
                              .GetAwaiter().GetResult()
                        ?? throw new KeyNotFoundException("Site not found.");

            if (entity.Status == "Active") return;

            entity.Status = "Active";

            _repo.UpdateAsync(entity, CancellationToken.None)
                 .GetAwaiter().GetResult();
        }

        // --- helpers ---

        private static SiteDto Map(Site s) => new()
        {
            SiteId = s.SiteId,
            Name = s.Name,
            AddressJson = s.AddressJson,
            Timezone = s.Timezone,
            Status = s.Status
        };

        private static void Validate(SiteCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Name is required.");

            if (string.IsNullOrWhiteSpace(dto.Timezone))
                dto.Timezone = "UTC";

            ValidateTimezone(dto.Timezone);
        }

        private static void ValidateTimezone(string tz)
        {
            // Accept only known system time zones
            if (!TimeZoneInfo.GetSystemTimeZones().Any(z => z.Id.Equals(tz, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException("Invalid timezone.");
        }

        private static string? NormalizeJson(string? json)
        {
            if (string.IsNullOrWhiteSpace(json)) return null;
            try
            {
                var doc = JsonDocument.Parse(json);
                return JsonSerializer.Serialize(doc.RootElement);
            }
            catch
            {
                // keep as-is if not valid JSON (admin free-form)
                return json;
            }
        }
    }
}