using Microsoft.AspNetCore.Mvc;
using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("rosters")]
    public class RostersController : ControllerBase
    {
        private readonly IRosterService _service;

        public RostersController(IRosterService service)
        {
            _service = service;
        }

        [HttpPost]
        public ActionResult<ApiResponse<RosterResponseDto>> Create([FromBody] CreateRosterDto dto)
        {
            var result = _service.CreateRoster(dto);
            return ApiResponse<RosterResponseDto>.Ok(result, "Roster created.");
        }

        [HttpPatch("{rosterId:int}/publish")]
        public ActionResult<ApiResponse<RosterResponseDto>> Publish(int rosterId, [FromBody] PublishRosterDto dto)
        {
            var result = _service.PublishRoster(rosterId, dto);
            return ApiResponse<RosterResponseDto>.Ok(result, "Roster published.");
        }
    }
}
