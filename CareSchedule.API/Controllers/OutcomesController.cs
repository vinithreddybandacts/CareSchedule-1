using Microsoft.AspNetCore.Mvc;
using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("outcome")]
    public class OutcomesController : ControllerBase
    {
        private readonly IOutcomeService _service;

        public OutcomesController(IOutcomeService service)
        {
            _service = service;
        }

        [HttpPost("{appointmentId:int}")]
        public ActionResult<ApiResponse<OutcomeResponseDto>> RecordOutcome(int appointmentId, [FromBody] RecordOutcomeRequestDto dto)
        {
            var result = _service.RecordOutcome(appointmentId, dto);
            return ApiResponse<OutcomeResponseDto>.Ok(result, "Outcome recorded.");
        }

        [HttpPost("{appointmentId:int}/no-show")]
        public ActionResult<ApiResponse<OutcomeResponseDto>> MarkNoShow(int appointmentId, [FromBody] RecordOutcomeRequestDto dto)
        {
            var result = _service.MarkNoShow(appointmentId, dto);
            return ApiResponse<OutcomeResponseDto>.Ok(result, "No-show recorded.");
        }
    }
}
