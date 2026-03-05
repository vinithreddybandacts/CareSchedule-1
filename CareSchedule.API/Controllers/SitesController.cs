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

        // GET /api/masterdata/sites
        
        [HttpGet]
        public IActionResult Search([FromQuery] SiteSearchQuery query)
        {
            var items = _service.SearchSite(query);
            return Ok(ApiResponse<object>.Ok(items));
        }

        // GET /api/masterdata/sites/{id}
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<SiteDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public ActionResult<ApiResponse<SiteDto>> Get(int id)
        {
            var site = _service.GetSite(id);
            return site is null
                ? NotFound(ApiResponse<object>.Fail(new { code = "RESOURCE_NOT_FOUND" }, "Site not found."))
                : Ok(ApiResponse<SiteDto>.Ok(site));
        }

        // POST /api/masterdata/sites
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<SiteDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public ActionResult<ApiResponse<SiteDto>> Create([FromBody] SiteCreateDto dto)
        {
            if (dto is null)
                return BadRequest(ApiResponse<object>.Fail(new { code = "BAD_REQUEST" }, "Request body is required."));

            try
            {
                var created = _service.CreateSite(dto);
                return CreatedAtAction(nameof(Get), new { id = created.SiteId },
                    ApiResponse<SiteDto>.Ok(created, "Site created."));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(new { code = "BAD_REQUEST" }, ex.Message));
            }
        }

        // PUT /api/masterdata/sites/{id}
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<SiteDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public ActionResult<ApiResponse<SiteDto>> Update(int id, [FromBody] SiteUpdateDto dto)
        {
            if (dto is null)
                return BadRequest(ApiResponse<object>.Fail(new { code = "BAD_REQUEST" }, "Request body is required."));

            try
            {
                var updated = _service.UpdateSite(id, dto);
                return Ok(ApiResponse<SiteDto>.Ok(updated, "Site updated."));
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

        // DELETE /api/masterdata/sites/{id} (soft: set Status -> Inactive)
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public ActionResult<ApiResponse<object>> DeactivateSite(int id)
        {
            return HandleStatusChange(id, _service.DeactivateSite, "Site deactivated.");
        }

        // POST /api/masterdata/sites/{id}/activate
        [HttpPost("{id:int}/activate")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public ActionResult<ApiResponse<object>> ActivateSite(int id)
        {
            return HandleStatusChange(id, _service.ActivateSite, "Site activated.");
        }

        // Helper to reduce duplication in Activate/Deactivate
        private ActionResult<ApiResponse<object>> HandleStatusChange(int id, Action<int> action, string message)
        {
            try
            {
                action(id);
                return Ok(ApiResponse<object>.Ok(new { id }, message));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse<object>.Fail(new { code = "RESOURCE_NOT_FOUND" }, "Site not found."));
            }
        }
    }
}