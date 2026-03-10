using Microsoft.AspNetCore.Mvc;
using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("roster-assignments")]
    public class RosterAssignmentsController : ControllerBase
    {
        private readonly IRosterService _service;

        public RosterAssignmentsController(IRosterService service)
        {
            _service = service;
        }

        [HttpPost]
        public ActionResult<ApiResponse<RosterAssignmentResponseDto>> Assign([FromBody] CreateRosterAssignmentDto dto)
        {
            var result = _service.AssignStaff(dto);
            return ApiResponse<RosterAssignmentResponseDto>.Ok(result, "Staff assigned.");
        }

        [HttpPatch("{id:int}/swap")]
        public ActionResult<ApiResponse<RosterAssignmentResponseDto>> Swap(int id, [FromBody] SwapAssignmentDto dto)
        {
            var result = _service.SwapShift(id, dto);
            return ApiResponse<RosterAssignmentResponseDto>.Ok(result, "Shift swapped.");
        }

        [HttpPatch("{id:int}/absent")]
        public ActionResult<ApiResponse<object>> MarkAbsent(int id)
        {
            _service.MarkAbsent(id);
            return ApiResponse<object>.Ok(new { id }, "Marked absent.");
        }

        [HttpGet]
        public ActionResult<ApiResponse<IEnumerable<RosterAssignmentResponseDto>>> Search([FromQuery] RosterAssignmentSearchDto dto)
        {
            var list = _service.SearchAssignments(dto);
            return ApiResponse<IEnumerable<RosterAssignmentResponseDto>>.Ok(list, "Roster assignments fetched.");
        }
    }
}
