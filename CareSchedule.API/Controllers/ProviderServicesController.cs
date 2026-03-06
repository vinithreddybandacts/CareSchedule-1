using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("api/masterdata")]
    [Produces("application/json")]
    public class ProviderServicesController : ControllerBase
    {
        private readonly IProviderServiceMappingService _service;

        public ProviderServicesController(IProviderServiceMappingService service)
        {
            _service = service;
        }

        [HttpPost("provider-services")]
        public IActionResult Assign([FromBody] ProviderServiceCreateDto dto)
        {
            if (dto is null)
                return BadRequest(ApiResponse<object>.Fail(new { code = "BAD_REQUEST" }, "Request body is required."));

            try
            {
                var created = _service.AssignServiceToProvider(dto);
                return CreatedAtAction(nameof(GetServicesByProvider), new { providerId = created.ProviderId },
                    ApiResponse<ProviderServiceDto>.Ok(created, "Service assigned to provider."));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(new { code = "BAD_REQUEST" }, ex.Message));
            }
        }

        [HttpGet("providers/{providerId:int}/services")]
        public IActionResult GetServicesByProvider(int providerId)
        {
            var items = _service.GetServicesByProvider(providerId);
            return Ok(ApiResponse<object>.Ok(items));
        }

        [HttpGet("services/{serviceId:int}/providers")]
        public IActionResult GetProvidersByService(int serviceId)
        {
            var items = _service.GetProvidersByService(serviceId);
            return Ok(ApiResponse<object>.Ok(items));
        }

        [HttpDelete("provider-services/{id:int}")]
        public IActionResult Remove(int id)
        {
            try
            {
                _service.RemoveMapping(id);
                return Ok(ApiResponse<object>.Ok(new { id }, "Mapping removed."));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse<object>.Fail(new { code = "RESOURCE_NOT_FOUND" }, "Mapping not found."));
            }
        }
    }
}
