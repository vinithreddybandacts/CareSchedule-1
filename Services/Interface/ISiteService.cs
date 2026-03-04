using CareSchedule.DTOs;

namespace CareSchedule.Services.Interface
{
    public interface ISiteService
    {
        Task<PagedResult<SiteDto>> SearchAsync(SiteSearchQuery query, CancellationToken ct = default);
        Task<SiteDto?> GetAsync(int id, CancellationToken ct = default);
        Task<SiteDto> CreateAsync(SiteCreateDto dto, CancellationToken ct = default);
        Task<SiteDto> UpdateAsync(int id, SiteUpdateDto dto, CancellationToken ct = default);
        Task DeactivateAsync(int id, CancellationToken ct = default);
        Task ActivateAsync(int id, CancellationToken ct = default);
    }
}
