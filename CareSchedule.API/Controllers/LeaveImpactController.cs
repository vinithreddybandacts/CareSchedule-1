using Microsoft.AspNetCore.Mvc;
using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("leave-impact")]
    public class LeaveImpactController(ILeaveService _leaveservice) : ControllerBase
    {
        [HttpPost]
        public ActionResult<ApiResponse<LeaveImpactResponseDto>> Create([FromBody] CreateLeaveImpactDto dto)
        {
            var result = _leaveservice.CreateImpact(dto);
            return ApiResponse<LeaveImpactResponseDto>.Ok(result, "Leave impact created.");
        }

        [HttpPatch("{impactId:int}/resolve")]
        public ActionResult<ApiResponse<LeaveImpactResponseDto>> Resolve(int impactId, [FromBody] ResolveLeaveImpactDto dto)
        {
            var result = _leaveservice.ResolveImpact(impactId, dto);
            return ApiResponse<LeaveImpactResponseDto>.Ok(result, "Leave impact resolved.");
        }
    }
}
