using CareSchedule.Models;

namespace CareSchedule.Repositories.Interface
{
    public interface IRoomRepository
    {
        Task<(List<Room> Items, int Total)> SearchAsync(
            string? RoomName,
            string? RoomType,
            string? status,
            int? siteId,
            int page,
            int pageSize,
            string? sortBy,
            string? sortDir,
            CancellationToken ct = default);

        Task<Room?> GetAsync(int id, CancellationToken ct = default);

        Task<Room> CreateAsync(Room entity, CancellationToken ct = default);

        Task UpdateAsync(Room entity, CancellationToken ct = default);
    }
}