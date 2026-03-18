using Microsoft.AspNetCore.Mvc;
using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("rosters")]
    public class RostersController(IRosterService _rosterservice) : ControllerBase
    {
        [HttpPost]
        public ActionResult<ApiResponse<RosterResponseDto>> Create([FromBody] CreateRosterDto dto)
        {
            var result = _rosterservice.CreateRoster(dto);
            return ApiResponse<RosterResponseDto>.Ok(result, "Roster created.");
        }

        [HttpPatch("{rosterId:int}/publish")]
        public ActionResult<ApiResponse<RosterResponseDto>> Publish(int rosterId, [FromBody] PublishRosterDto dto)
        {
            var result = _rosterservice.PublishRoster(rosterId, dto);
            return ApiResponse<RosterResponseDto>.Ok(result, "Roster published.");
        }
    }
}
