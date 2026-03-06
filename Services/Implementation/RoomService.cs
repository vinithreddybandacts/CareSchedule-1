using System.Text.Json;
using CareSchedule.DTOs;
using CareSchedule.Models;
using CareSchedule.Repositories.Interface;
using CareSchedule.Services.Interface;
using CareSchedule.Shared.Exceptions;

namespace CareSchedule.Services.Implementation
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _rooms;
        private readonly ISiteRepository _sites;

        
        public RoomService(IRoomRepository rooms, ISiteRepository sites)
        {
            _rooms = rooms;
            _sites = sites;
        }

        public List<RoomDto> SearchRoom(RoomSearchQuery query)
        {
            var (items, _) = _rooms.SearchAsync(
                RoomName: query.RoomName,
                RoomType: query.RoomType,
                status: query.Status,
                siteId: query.SiteId,
                page: query.Page <= 0 ? 1 : query.Page,
                pageSize: query.PageSize <= 0 ? 25 : query.PageSize,
                sortBy: query.SortBy,
                sortDir: query.SortDir,
                ct: CancellationToken.None
            ).GetAwaiter().GetResult();

            return items.Select(Map).ToList();
        }

        public RoomDto? GetRoom(int id)
        {
            var room = _rooms.GetAsync(id, CancellationToken.None)
                            .GetAwaiter().GetResult();
            return room is null ? null : Map(room);
        }

        public RoomDto CreateRoom(RoomCreateDto dto)
        {
            // 1) Validate basic input
            if (string.IsNullOrWhiteSpace(dto.RoomName))
                throw new RoomNameRequiredException();

            if (dto.SiteId <= 0)
                throw new SiteIdRequiredException();

            // 2) Ensure the referenced Site exists
            var site = _sites.GetAsync(dto.SiteId, CancellationToken.None)
                            .GetAwaiter().GetResult();
            if (site is null)
                throw new SiteNotFoundForRoomException();

            // 3) (Optional) Enforce uniqueness: RoomName per Site
            var (existing, _) = _rooms.SearchAsync(
                RoomName: dto.RoomName,
                RoomType: dto.RoomType,
                status: null,
                siteId: dto.SiteId,
                page: 1,
                pageSize: 1,
                sortBy: null,
                sortDir: null,
                ct: CancellationToken.None
            ).GetAwaiter().GetResult();
            if (existing.Any())
                throw new DuplicateRoomNameException();

            // 4) Create the entity
            var entity = new Room
            {
                RoomName = dto.RoomName.Trim(),
                RoomType = dto.RoomType.Trim(),
                SiteId = dto.SiteId,
                AttributesJson = NormalizeJson(dto.AttributesJson),
                Status = "Active"
            };

            // 5) Persist and map
            entity = _rooms.CreateAsync(entity, CancellationToken.None)
                        .GetAwaiter().GetResult();

            return Map(entity);
        }
        public RoomDto UpdateRoom(int id, RoomUpdateDto dto)
        {
            
            var entity = _rooms.GetAsync(id, CancellationToken.None)
                              .GetAwaiter().GetResult()
                        ?? throw new KeyNotFoundException("Room not found.");

            if (!string.IsNullOrWhiteSpace(dto.RoomName))
                entity.RoomName = dto.RoomName.Trim();

            if (dto.SiteId.HasValue)
                entity.SiteId = dto.SiteId.Value;

            if (dto.AttributesJson is not null)
                entity.AttributesJson = NormalizeJson(dto.AttributesJson);

            // inside UpdateRoom, in the block that handles dto.SiteId:
            if (dto.SiteId.HasValue)
            {
                var site = _sites.GetAsync(dto.SiteId.Value, CancellationToken.None)
                                .GetAwaiter().GetResult();
                if (site is null)
                    throw new KeyNotFoundException("Site not found.");

                entity.SiteId = dto.SiteId.Value;
            }

            _rooms.UpdateAsync(entity, CancellationToken.None)
                 .GetAwaiter().GetResult();

            return Map(entity);
        }

        public void DeactivateRoom(int id)
        {
            var entity = _rooms.GetAsync(id, CancellationToken.None)
                              .GetAwaiter().GetResult()
                        ?? throw new KeyNotFoundException("Room not found.");

            if (string.Equals(entity.Status, "Inactive", StringComparison.Ordinal))
                return;

            entity.Status = "Inactive";

            _rooms.UpdateAsync(entity, CancellationToken.None)
                 .GetAwaiter().GetResult();
        }

        public void ActivateRoom(int id)
        {
            var entity = _rooms.GetAsync(id, CancellationToken.None)
                              .GetAwaiter().GetResult()
                        ?? throw new KeyNotFoundException("Room not found.");

            if (string.Equals(entity.Status, "Active", StringComparison.Ordinal))
                return;

            entity.Status = "Active";

            _rooms.UpdateAsync(entity, CancellationToken.None)
                 .GetAwaiter().GetResult();
        }

        // --- helpers ---

        private static RoomDto Map(Room r) => new()
        {
            RoomId = r.RoomId,
            RoomName = r.RoomName,
            SiteId = r.SiteId,
            AttributesJson = r.AttributesJson,
            Status = r.Status
        };

        private static void Validate(RoomCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.RoomName))
                throw new ArgumentException("RoomName is required.");

            if (dto.SiteId <= 0)
                throw new ArgumentException("SiteId is required.");
        }

        private static string? NormalizeJson(string? json)
        {
            if (string.IsNullOrWhiteSpace(json)) return null;
            try
            {
                using var doc = JsonDocument.Parse(json);
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