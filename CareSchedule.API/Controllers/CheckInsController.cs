using Microsoft.AspNetCore.Mvc;
using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    public class CheckInsController : ControllerBase
    {
        private readonly ICheckInService _service;

        public CheckInsController(ICheckInService service)
        {
            _service = service;
        }

        [HttpPost("checkin/{appointmentId:int}")]
        public ActionResult<ApiResponse<CheckInResponseDto>> CheckIn(int appointmentId, [FromBody] CreateCheckInRequestDto dto)
        {
            var result = _service.CheckIn(appointmentId, dto);
            return ApiResponse<CheckInResponseDto>.Ok(result, "Patient checked in.");
        }

        [HttpPatch("checkins/{checkInId:int}/assign-room")]
        public ActionResult<ApiResponse<CheckInResponseDto>> AssignRoom(int checkInId, [FromBody] AssignRoomRequestDto dto)
        {
            var result = _service.AssignRoom(checkInId, dto);
            return ApiResponse<CheckInResponseDto>.Ok(result, "Room assigned.");
        }

        [HttpPatch("checkins/{checkInId:int}/in-room")]
        public ActionResult<ApiResponse<CheckInResponseDto>> MoveToRoom(int checkInId)
        {
            var result = _service.MoveToRoom(checkInId);
            return ApiResponse<CheckInResponseDto>.Ok(result, "Patient moved to room.");
        }

        [HttpPatch("checkins/{checkInId:int}/status")]
        public ActionResult<ApiResponse<CheckInResponseDto>> UpdateStatus(int checkInId, [FromBody] UpdateCheckInStatusDto dto)
        {
            var result = _service.UpdateStatus(checkInId, dto);
            return ApiResponse<CheckInResponseDto>.Ok(result, "Check-in status updated.");
        }

        [HttpPatch("checkins/{checkInId:int}/with-provider")]
        public ActionResult<ApiResponse<CheckInResponseDto>> StartConsultation(int checkInId)
        {
            var result = _service.StartConsultation(checkInId);
            return ApiResponse<CheckInResponseDto>.Ok(result, "Consultation started.");
        }

        [HttpGet("checkins")]
        public ActionResult<ApiResponse<IEnumerable<CheckInResponseDto>>> Search([FromQuery] CheckInSearchDto dto)
        {
            var list = _service.Search(dto);
            return ApiResponse<IEnumerable<CheckInResponseDto>>.Ok(list, "Check-ins fetched.");
        }
    }
}
