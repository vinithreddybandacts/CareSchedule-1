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
            var created = _service.AssignServiceToProvider(dto);
            return CreatedAtAction(nameof(GetServicesByProvider), new { providerId = created.ProviderId },
                ApiResponse<ProviderServiceDto>.Ok(created, "Service assigned to provider."));
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
            _service.RemoveMapping(id);
            return Ok(ApiResponse<object>.Ok(new { id }, "Mapping removed."));
        }
    }
}

