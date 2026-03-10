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
    }
}
