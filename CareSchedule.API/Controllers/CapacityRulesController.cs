using Microsoft.AspNetCore.Mvc;
using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("capacity-rules")]
    public class CapacityRulesController : ControllerBase
    {
        private readonly IRulesService _service;

        public CapacityRulesController(IRulesService service)
        {
            _service = service;
        }

        [HttpPost]
        public ActionResult<ApiResponse<CapacityRuleResponseDto>> Create([FromBody] CreateCapacityRuleDto dto)
        {
            var result = _service.CreateCapacityRule(dto);
            return ApiResponse<CapacityRuleResponseDto>.Ok(result, "Capacity rule created.");
        }

        [HttpPut("{ruleId:int}")]
        public ActionResult<ApiResponse<CapacityRuleResponseDto>> Update(int ruleId, [FromBody] UpdateCapacityRuleDto dto)
        {
            var result = _service.UpdateCapacityRule(ruleId, dto);
            return ApiResponse<CapacityRuleResponseDto>.Ok(result, "Capacity rule updated.");
        }
    }
}
