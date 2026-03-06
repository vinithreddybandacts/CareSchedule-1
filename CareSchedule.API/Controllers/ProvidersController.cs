using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("api/masterdata/providers")]
    [Produces("application/json")]
    public class ProvidersController : ControllerBase
    {
        private readonly IProviderMasterService _service;

        public ProvidersController(IProviderMasterService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var items = _service.GetAllProviders();
            return Ok(ApiResponse<object>.Ok(items));
        }

        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            var provider = _service.GetProvider(id);
            return provider is null
                ? NotFound(ApiResponse<object>.Fail(new { code = "RESOURCE_NOT_FOUND" }, "Provider not found."))
                : Ok(ApiResponse<ProviderDto>.Ok(provider));
        }

        [HttpPost]
        public IActionResult Create([FromBody] ProviderCreateDto dto)
        {
            if (dto is null)
                return BadRequest(ApiResponse<object>.Fail(new { code = "BAD_REQUEST" }, "Request body is required."));

            try
            {
                var created = _service.CreateProvider(dto);
                return CreatedAtAction(nameof(Get), new { id = created.ProviderId },
                    ApiResponse<ProviderDto>.Ok(created, "Provider created."));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(new { code = "BAD_REQUEST" }, ex.Message));
            }
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] ProviderUpdateDto dto)
        {
            if (dto is null)
                return BadRequest(ApiResponse<object>.Fail(new { code = "BAD_REQUEST" }, "Request body is required."));

            try
            {
                var updated = _service.UpdateProvider(id, dto);
                return Ok(ApiResponse<ProviderDto>.Ok(updated, "Provider updated."));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse<object>.Fail(new { code = "RESOURCE_NOT_FOUND" }, "Provider not found."));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(new { code = "BAD_REQUEST" }, ex.Message));
            }
        }

        [HttpDelete("{id:int}")]
        public IActionResult Deactivate(int id)
        {
            return HandleStatusChange(id, _service.DeactivateProvider, "Provider deactivated.");
        }

        [HttpPost("{id:int}/activate")]
        public IActionResult Activate(int id)
        {
            return HandleStatusChange(id, _service.ActivateProvider, "Provider activated.");
        }

        private IActionResult HandleStatusChange(int id, Action<int> action, string message)
        {
            try
            {
                action(id);
                return Ok(ApiResponse<object>.Ok(new { id }, message));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse<object>.Fail(new { code = "RESOURCE_NOT_FOUND" }, "Provider not found."));
            }
        }
    }
}
