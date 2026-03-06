using CareSchedule.Infrastructure.Data;
using CareSchedule.Models;
using CareSchedule.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace CareSchedule.Repositories.Implementation
{
    public class RoomRepository : IRoomRepository
    {
        private readonly CareScheduleContext _db;

        public RoomRepository(CareScheduleContext db)
        {
            _db = db;
        }

        public async Task<(List<Room> Items, int Total)> SearchAsync(
            string? RoomName,
            string? RoomType,
            string? status,
            int? siteId,
            int page,
            int pageSize,
            string? sortBy,
            string? sortDir,
            CancellationToken ct = default)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 25;

            var query = _db.Rooms.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(RoomName))
            {
                var pattern = RoomName.Trim();
                query = query.Where(r => EF.Functions.Like(r.RoomName, $"%{pattern}%"));
            }

            if (!string.IsNullOrWhiteSpace(RoomType))
            {
                var pattern = RoomType.Trim();
                query = query.Where(r => EF.Functions.Like(r.RoomType, $"%{pattern}%"));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                var s = status.Trim();
                query = query.Where(r => r.Status == s);
            }

            if (siteId.HasValue)
            {
                query = query.Where(r => r.SiteId == siteId.Value);
            }

            // Sorting
            bool desc = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);
            query = (sortBy?.ToLowerInvariant()) switch
            {
                "RoomName"     => desc ? query.OrderByDescending(r => r.RoomName)     : query.OrderBy(r => r.RoomName),
                "status"   => desc ? query.OrderByDescending(r => r.Status)   : query.OrderBy(r => r.Status),
                "siteid"   => desc ? query.OrderByDescending(r => r.SiteId)   : query.OrderBy(r => r.SiteId),
                _          => query.OrderBy(r => r.RoomName)
            };

            var total = await query.CountAsync(ct);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, total);
        }

        public async Task<Room?> GetAsync(int id, CancellationToken ct = default)
        {
            return await _db.Rooms.AsNoTracking()
                .FirstOrDefaultAsync(r => r.RoomId == id, ct);
        }

        public async Task<Room> CreateAsync(Room entity, CancellationToken ct = default)
        {
            _db.Rooms.Add(entity);
            await _db.SaveChangesAsync(ct);
            return entity;
        }

        public async Task UpdateAsync(Room entity, CancellationToken ct = default)
        {
            _db.Rooms.Update(entity);
            await _db.SaveChangesAsync(ct);
        }
    }
}