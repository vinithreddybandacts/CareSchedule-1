using Microsoft.AspNetCore.Mvc;
using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("oncall")]
    public class OnCallController(IRosterService _rosterservice) : ControllerBase
    {
        [HttpPost]
        public ActionResult<ApiResponse<OnCallResponseDto>> Create([FromBody] CreateOnCallDto dto)
        {
            var result = _rosterservice.CreateOnCall(dto);
            return ApiResponse<OnCallResponseDto>.Ok(result, "On-call coverage created.");
        }

        [HttpPut("{id:int}")]
        public ActionResult<ApiResponse<OnCallResponseDto>> Update(int id, [FromBody] UpdateOnCallDto dto)
        {
            var result = _rosterservice.UpdateOnCall(id, dto);
            return ApiResponse<OnCallResponseDto>.Ok(result, "On-call coverage updated.");
        }
    }
}
