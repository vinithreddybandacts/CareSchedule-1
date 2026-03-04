using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("api/masterdata/sites")]
    public class SitesController : ControllerBase
    {
        private readonly ISiteService _service;

        public SitesController(ISiteService service)
        {
            _service = service;
        }

        // GET /api/masterdata/sites?name=&status=&page=&pageSize=&sortBy=&sortDir=
        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] SiteSearchQuery query, CancellationToken ct)
        {
            var result = await _service.SearchAsync(query, ct);
            return Ok(ApiResponse<object>.Ok(new
            {
                items = result.Items,
                total = result.Total,
                page = result.Page,
                pageSize = result.PageSize
            }));
        }

        // GET /api/masterdata/sites/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id, CancellationToken ct)
        {
            var site = await _service.GetAsync(id, ct);
            if (site is null) return NotFound(ApiResponse<object>.Fail(new { code = "RESOURCE_NOT_FOUND" }, "Site not found."));
            return Ok(ApiResponse<object>.Ok(site));
        }

        // POST /api/masterdata/sites
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SiteCreateDto dto, CancellationToken ct)
        {
            try
            {
                var created = await _service.CreateAsync(dto, ct);
                return CreatedAtAction(nameof(Get), new { id = created.SiteId }, ApiResponse<object>.Ok(created, "Site created."));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(new { code = "BAD_REQUEST" }, ex.Message));
            }
        }

        // PUT /api/masterdata/sites/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] SiteUpdateDto dto, CancellationToken ct)
        {
            try
            {
                var updated = await _service.UpdateAsync(id, dto, ct);
                return Ok(ApiResponse<object>.Ok(updated, "Site updated."));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse<object>.Fail(new { code = "RESOURCE_NOT_FOUND" }, "Site not found."));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(new { code = "BAD_REQUEST" }, ex.Message));
            }
        }

        // DELETE /api/masterdata/sites/{id}   (soft: Status -> Inactive)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Deactivate(int id, CancellationToken ct)
        {
            try
            {
                await _service.DeactivateAsync(id, ct);
                return Ok(ApiResponse<object>.Ok(new { id }, "Site deactivated."));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse<object>.Fail(new { code = "RESOURCE_NOT_FOUND" }, "Site not found."));
            }
        }

        // POST /api/masterdata/sites/{id}/activate
        [HttpPost("{id:int}/activate")]
        public async Task<IActionResult> Activate(int id, CancellationToken ct)
        {
            try
            {
                await _service.ActivateAsync(id, ct);
                return Ok(ApiResponse<object>.Ok(new { id }, "Site activated."));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse<object>.Fail(new { code = "RESOURCE_NOT_FOUND" }, "Site not found."));
            }
        }
    }
}
