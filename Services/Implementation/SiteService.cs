using System.Text.Json;
using CareSchedule.DTOs;
using CareSchedule.Models;
using CareSchedule.Repositories.Interface;
using CareSchedule.Services.Interface;

namespace CareSchedule.Services
{
    public class SiteService : ISiteService
    {
        private readonly ISiteRepository _repo;

        public SiteService(ISiteRepository repo)
        {
            _repo = repo;
        }

        public async Task<PagedResult<SiteDto>> SearchAsync(SiteSearchQuery query, CancellationToken ct = default)
        {
            var (items, total) = await _repo.SearchAsync(
                query.Name, query.Status,
                query.Page, query.PageSize,
                query.SortBy, query.SortDir, ct);

            return new PagedResult<SiteDto>
            {
                Items = items.Select(Map).ToList(),
                Total = total,
                Page = query.Page <= 0 ? 1 : query.Page,
                PageSize = query.PageSize <= 0 ? 25 : query.PageSize
            };
        }

        public async Task<SiteDto?> GetAsync(int id, CancellationToken ct = default)
        {
            var site = await _repo.GetAsync(id, ct);
            return site is null ? null : Map(site);
        }

        public async Task<SiteDto> CreateAsync(SiteCreateDto dto, CancellationToken ct = default)
        {
            Validate(dto);

            var entity = new Site
            {
                Name = dto.Name.Trim(),
                AddressJson = NormalizeJson(dto.AddressJson),
                Timezone = dto.Timezone,
                Status = "Active"
            };

            entity = await _repo.CreateAsync(entity, ct);
            return Map(entity);
        }

        public async Task<SiteDto> UpdateAsync(int id, SiteUpdateDto dto, CancellationToken ct = default)
        {
            var entity = await _repo.GetAsync(id, ct) ?? throw new KeyNotFoundException("Site not found.");

            if (!string.IsNullOrWhiteSpace(dto.Name))
                entity.Name = dto.Name.Trim();

            if (dto.AddressJson is not null)
                entity.AddressJson = NormalizeJson(dto.AddressJson);

            if (!string.IsNullOrWhiteSpace(dto.Timezone))
            {
                ValidateTimezone(dto.Timezone);
                entity.Timezone = dto.Timezone;
            }

            await _repo.UpdateAsync(entity, ct);
            return Map(entity);
        }

        public async Task DeactivateAsync(int id, CancellationToken ct = default)
        {
            var entity = await _repo.GetAsync(id, ct) ?? throw new KeyNotFoundException("Site not found.");
            if (entity.Status == "Inactive") return;
            entity.Status = "Inactive";
            await _repo.UpdateAsync(entity, ct);
        }

        public async Task ActivateAsync(int id, CancellationToken ct = default)
        {
            var entity = await _repo.GetAsync(id, ct) ?? throw new KeyNotFoundException("Site not found.");
            if (entity.Status == "Active") return;
            entity.Status = "Active";
            await _repo.UpdateAsync(entity, ct);
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