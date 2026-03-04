using CareSchedule.Models;

namespace CareSchedule.Repositories.Interface
{
    public interface ISiteRepository
    {
        Task<(IReadOnlyList<Site> Items, int Total)> SearchAsync(
            string? name, string? status,
            int page, int pageSize,
            string sortBy, string sortDir,
            CancellationToken ct = default);

        Task<Site?> GetAsync(int id, CancellationToken ct = default);
        Task<Site> CreateAsync(Site entity, CancellationToken ct = default);
        Task UpdateAsync(Site entity, CancellationToken ct = default);
    }
}