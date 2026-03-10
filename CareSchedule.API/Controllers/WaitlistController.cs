using Microsoft.AspNetCore.Mvc;
using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("waitlist")]
    public class WaitlistController : ControllerBase
    {
        private readonly IWaitlistService _service;

        public WaitlistController(IWaitlistService service)
        {
            _service = service;
        }

        [HttpPost]
        public ActionResult<ApiResponse<WaitlistResponseDto>> Add([FromBody] CreateWaitlistRequestDto dto)
        {
            var result = _service.Add(dto);
            return ApiResponse<WaitlistResponseDto>.Ok(result, "Added to waitlist.");
        }

        [HttpDelete("{waitId:int}")]
        public ActionResult<ApiResponse<object>> Remove(int waitId)
        {
            _service.Remove(waitId);
            return ApiResponse<object>.Ok(null, "Removed from waitlist.");
        }

        [HttpGet]
        public ActionResult<ApiResponse<IEnumerable<WaitlistResponseDto>>> Search([FromQuery] WaitlistSearchDto dto)
        {
            var list = _service.Search(dto);
            return ApiResponse<IEnumerable<WaitlistResponseDto>>.Ok(list, "Waitlist fetched.");
        }

        [HttpPatch("{waitId:int}/filled")]
        public ActionResult<ApiResponse<WaitlistResponseDto>> Fill(int waitId, [FromBody] FillWaitlistRequestDto dto)
        {
            var result = _service.Fill(waitId, dto);
            return ApiResponse<WaitlistResponseDto>.Ok(result, "Waitlist slot filled.");
        }
    }
}
