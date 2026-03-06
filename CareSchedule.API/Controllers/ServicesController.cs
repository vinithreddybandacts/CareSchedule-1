using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("api/masterdata/services")]
    [Produces("application/json")]
    public class ServicesController : ControllerBase
    {
        private readonly IServiceMasterService _service;

        public ServicesController(IServiceMasterService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var items = _service.GetAllServices();
            return Ok(ApiResponse<object>.Ok(items));
        }

        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            var svc = _service.GetService(id);
            return svc is null
                ? NotFound(ApiResponse<object>.Fail(new { code = "RESOURCE_NOT_FOUND" }, "Service not found."))
                : Ok(ApiResponse<ServiceDto>.Ok(svc));
        }

        [HttpPost]
        public IActionResult Create([FromBody] ServiceCreateDto dto)
        {
            if (dto is null)
                return BadRequest(ApiResponse<object>.Fail(new { code = "BAD_REQUEST" }, "Request body is required."));

            try
            {
                var created = _service.CreateService(dto);
                return CreatedAtAction(nameof(Get), new { id = created.ServiceId },
                    ApiResponse<ServiceDto>.Ok(created, "Service created."));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(new { code = "BAD_REQUEST" }, ex.Message));
            }
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] ServiceUpdateDto dto)
        {
            if (dto is null)
                return BadRequest(ApiResponse<object>.Fail(new { code = "BAD_REQUEST" }, "Request body is required."));

            try
            {
                var updated = _service.UpdateService(id, dto);
                return Ok(ApiResponse<ServiceDto>.Ok(updated, "Service updated."));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse<object>.Fail(new { code = "RESOURCE_NOT_FOUND" }, "Service not found."));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(new { code = "BAD_REQUEST" }, ex.Message));
            }
        }

        [HttpDelete("{id:int}")]
        public IActionResult Deactivate(int id)
        {
            return HandleStatusChange(id, _service.DeactivateService, "Service deactivated.");
        }

        [HttpPost("{id:int}/activate")]
        public IActionResult Activate(int id)
        {
            return HandleStatusChange(id, _service.ActivateService, "Service activated.");
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
                return NotFound(ApiResponse<object>.Fail(new { code = "RESOURCE_NOT_FOUND" }, "Service not found."));
            }
        }
    }
}
