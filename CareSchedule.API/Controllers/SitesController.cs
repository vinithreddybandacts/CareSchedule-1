using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("api/masterdata/sites")]
    [Produces("application/json")]
    public class SitesController : ControllerBase
    {
        private readonly ISiteService _service;

        public SitesController(ISiteService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult Search([FromQuery] SiteSearchQuery query)
        {
            var items = _service.SearchSite(query);
            return Ok(ApiResponse<object>.Ok(items));
        }

        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            var site = _service.GetSite(id);
            return site is null
                ? NotFound(ApiResponse<object>.Fail(new { code = "RESOURCE_NOT_FOUND" }, "Site not found."))
                : Ok(ApiResponse<SiteDto>.Ok(site));
        }

        [HttpPost]
        public IActionResult Create([FromBody] SiteCreateDto dto)
        {
            var created = _service.CreateSite(dto);
            return CreatedAtAction(nameof(Get), new { id = created.SiteId },
                ApiResponse<SiteDto>.Ok(created, "Site created."));
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] SiteUpdateDto dto)
        {
            var updated = _service.UpdateSite(id, dto);
            return Ok(ApiResponse<SiteDto>.Ok(updated, "Site updated."));
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeactivateSite(int id)
        {
            _service.DeactivateSite(id);
            return Ok(ApiResponse<object>.Ok(new { id }, "Site deactivated."));
        }

        [HttpPost("{id:int}/activate")]
        public IActionResult ActivateSite(int id)
        {
            _service.ActivateSite(id);
            return Ok(ApiResponse<object>.Ok(new { id }, "Site activated."));
        }
    }
}
