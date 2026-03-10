using Microsoft.AspNetCore.Mvc;
using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("leave")]
    public class LeaveController : ControllerBase
    {
        private readonly ILeaveService _service;

        public LeaveController(ILeaveService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<ApiResponse<IEnumerable<LeaveRequestResponseDto>>> Search([FromQuery] LeaveSearchDto dto)
        {
            var list = _service.Search(dto);
            return ApiResponse<IEnumerable<LeaveRequestResponseDto>>.Ok(list, "Leave requests fetched.");
        }

        [HttpPatch("{leaveId:int}/approve")]
        public ActionResult<ApiResponse<LeaveRequestResponseDto>> Approve(int leaveId)
        {
            var result = _service.Approve(leaveId);
            return ApiResponse<LeaveRequestResponseDto>.Ok(result, "Leave approved.");
        }

        [HttpPatch("{leaveId:int}/reject")]
        public ActionResult<ApiResponse<LeaveRequestResponseDto>> Reject(int leaveId)
        {
            var result = _service.Reject(leaveId);
            return ApiResponse<LeaveRequestResponseDto>.Ok(result, "Leave rejected.");
        }
    }
}
