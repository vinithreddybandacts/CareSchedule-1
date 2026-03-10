using Microsoft.AspNetCore.Mvc;
using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("oncall")]
    public class OnCallController : ControllerBase
    {
        private readonly IRosterService _service;

        public OnCallController(IRosterService service)
        {
            _service = service;
        }

        [HttpPost]
        public ActionResult<ApiResponse<OnCallResponseDto>> Create([FromBody] CreateOnCallDto dto)
        {
            var result = _service.CreateOnCall(dto);
            return ApiResponse<OnCallResponseDto>.Ok(result, "On-call coverage created.");
        }

        [HttpPut("{id:int}")]
        public ActionResult<ApiResponse<OnCallResponseDto>> Update(int id, [FromBody] UpdateOnCallDto dto)
        {
            var result = _service.UpdateOnCall(id, dto);
            return ApiResponse<OnCallResponseDto>.Ok(result, "On-call coverage updated.");
        }
    }
}
