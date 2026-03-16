using Microsoft.AspNetCore.Mvc;
using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("sla")]
    public class SlaController : ControllerBase
    {
        private readonly IRulesService _service;

        public SlaController(IRulesService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<ApiResponse<IEnumerable<SlaResponseDto>>> Search(
            [FromQuery] string? scope, [FromQuery] string? status)
        {
            var list = _service.SearchSlas(scope, status);
            return ApiResponse<IEnumerable<SlaResponseDto>>.Ok(list, "SLAs fetched.");
        }

        [HttpGet("{slaId:int}")]
        public ActionResult<ApiResponse<SlaResponseDto>> Get(int slaId)
        {
            var result = _service.GetSla(slaId);
            return ApiResponse<SlaResponseDto>.Ok(result);
        }

        [HttpPost]
        public ActionResult<ApiResponse<SlaResponseDto>> Create([FromBody] CreateSlaDto dto)
        {
            var result = _service.CreateSla(dto);
            return ApiResponse<SlaResponseDto>.Ok(result, "SLA created.");
        }

        [HttpPut("{slaId:int}")]
        public ActionResult<ApiResponse<SlaResponseDto>> Update(int slaId, [FromBody] UpdateSlaDto dto)
        {
            var result = _service.UpdateSla(slaId, dto);
            return ApiResponse<SlaResponseDto>.Ok(result, "SLA updated.");
        }

        [HttpDelete("{slaId:int}")]
        public ActionResult<ApiResponse<object>> Deactivate(int slaId)
        {
            _service.DeactivateSla(slaId);
            return ApiResponse<object>.Ok(new { slaId }, "SLA deactivated.");
        }
    }
}