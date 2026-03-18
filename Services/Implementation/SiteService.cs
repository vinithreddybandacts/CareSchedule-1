using CareSchedule.DTOs;
using CareSchedule.Models;
using CareSchedule.Repositories.Interface;
using CareSchedule.Services.Interface;
using System.Collections.Generic;

namespace CareSchedule.Services.Implementation
{
    public class SiteService(ISiteRepository _siterepo) : ISiteService
    {
        public List<SiteDto> SearchSite(SiteSearchQuery q)
        {
            var (items, _) = _siterepo.Search(
                q.Name,
                q.Status,
                q.Page <= 0 ? 1 : q.Page,
                q.PageSize <= 0 ? 25 : q.PageSize,
                sortBy: string.IsNullOrWhiteSpace(q.SortBy) ? "name" : q.SortBy,
                sortDir: string.IsNullOrWhiteSpace(q.SortDir) ? "asc" : q.SortDir
            );

            var list = new List<SiteDto>(items.Count);
            foreach (var s in items) list.Add(Map(s));
            return list;
        }

        public SiteDto GetSite(int id)
        {
            var s = _siterepo.Get(id);
            if (s is null) throw new KeyNotFoundException("Site not found.");
            return Map(s);
        }

        public SiteDto CreateSite(SiteCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name)) throw new ArgumentException("Name is required.");
            var e = new Site
            {
                Name = dto.Name.Trim(),
                AddressJson = dto.AddressJson,
                Timezone = string.IsNullOrWhiteSpace(dto.Timezone) ? "UTC" : dto.Timezone.Trim(),
                Status = "Active"
            };
            e = _siterepo.Create(e);
            return Map(e);
        }

        public SiteDto UpdateSite(int id, SiteUpdateDto dto)
        {
            var e = _siterepo.Get(id);
            if (e is null) throw new KeyNotFoundException("Site not found.");

            if (!string.IsNullOrWhiteSpace(dto.Name)) e.Name = dto.Name.Trim();
            if (dto.AddressJson is not null) e.AddressJson = dto.AddressJson;
            if (!string.IsNullOrWhiteSpace(dto.Timezone)) e.Timezone = dto.Timezone.Trim();

            _siterepo.Update(e);
            return Map(e);
        }

        public void DeactivateSite(int id)
        {
            var e = _siterepo.Get(id);
            if (e is null) throw new KeyNotFoundException("Site not found.");
            if (e.Status != "Inactive") { e.Status = "Inactive"; _siterepo.Update(e); }
        }

        public void ActivateSite(int id)
        {
            var e = _siterepo.Get(id);
            if (e is null) throw new KeyNotFoundException("Site not found.");
            if (e.Status != "Active") { e.Status = "Active"; _siterepo.Update(e); }
        }

        private static SiteDto Map(Site s) => new()
        {
            SiteId = s.SiteId,
            Name = s.Name,
            AddressJson = s.AddressJson,
            Timezone = s.Timezone,
            Status = s.Status
        };
    }
}