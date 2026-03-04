using CareSchedule.Models;
using CareSchedule.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using CareSchedule.Infrastructure.Data;

namespace CareSchedule.Repositories.Implementation
{
    public class SiteRepository : ISiteRepository
    {
        private readonly CareScheduleContext _db;

        public SiteRepository(CareScheduleContext db)
        {
            _db = db;
        }

        public async Task<(IReadOnlyList<Site> Items, int Total)> SearchAsync(
            string? name, string? status,
            int page, int pageSize,
            string sortBy, string sortDir,
            CancellationToken ct = default)
        {
            var q = _db.Sites.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                var like = $"%{name.Trim().ToLower()}%";
                q = q.Where(s => EF.Functions.Like(s.Name.ToLower(), like));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                q = q.Where(s => s.Status == status);
            }

            var asc = !string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);
            q = (sortBy?.ToLower()) switch
            {
                "timezone" => asc ? q.OrderBy(x => x.Timezone) : q.OrderByDescending(x => x.Timezone),
                "status"   => asc ? q.OrderBy(x => x.Status)   : q.OrderByDescending(x => x.Status),
                _          => asc ? q.OrderBy(x => x.Name)     : q.OrderByDescending(x => x.Name),
            };

            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 25;

            var total = await q.CountAsync(ct);
            var items = await q.Skip((page - 1) * pageSize)
                               .Take(pageSize)
                               .ToListAsync(ct);

            return (items, total);
        }

        public Task<Site?> GetAsync(int id, CancellationToken ct = default)
            => _db.Sites.AsNoTracking().FirstOrDefaultAsync(s => s.SiteId == id, ct);

        public async Task<Site> CreateAsync(Site entity, CancellationToken ct = default)
        {
            _db.Sites.Add(entity);
            await _db.SaveChangesAsync(ct);
            return entity;
        }

        public async Task UpdateAsync(Site entity, CancellationToken ct = default)
        {
            _db.Sites.Update(entity);
            await _db.SaveChangesAsync(ct);
        }
    }
}
