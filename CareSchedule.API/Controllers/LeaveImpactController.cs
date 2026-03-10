using Microsoft.AspNetCore.Mvc;
using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("leave-impact")]
    public class LeaveImpactController : ControllerBase
    {
        private readonly ILeaveService _service;

        public LeaveImpactController(ILeaveService service)
        {
            _service = service;
        }

        [HttpPost]
        public ActionResult<ApiResponse<LeaveImpactResponseDto>> Create([FromBody] CreateLeaveImpactDto dto)
        {
            var result = _service.CreateImpact(dto);
            return ApiResponse<LeaveImpactResponseDto>.Ok(result, "Leave impact created.");
        }

        [HttpPatch("{impactId:int}/resolve")]
        public ActionResult<ApiResponse<LeaveImpactResponseDto>> Resolve(int impactId, [FromBody] ResolveLeaveImpactDto dto)
        {
            var result = _service.ResolveImpact(impactId, dto);
            return ApiResponse<LeaveImpactResponseDto>.Ok(result, "Leave impact resolved.");
        }
    }
}
