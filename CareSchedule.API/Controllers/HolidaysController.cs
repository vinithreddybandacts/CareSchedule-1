using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("api/admin/holidays")]
    [Produces("application/json")]
    public class HolidaysController(IHolidayService _holidayservice) : ControllerBase
    {
        // GET /api/admin/holidays
        [HttpGet]
        public IActionResult Search([FromQuery] HolidaySearchQuery query)
        {
            var items = _holidayservice.SearchHoliday(query);
            return Ok(ApiResponse<object>.Ok(items));
        }

        // GET /api/admin/holidays/{id}
        [HttpGet("{id:int}")]
        public ActionResult<ApiResponse<HolidayDto>> Get(int id)
        {
            var holiday = _holidayservice.GetHoliday(id);        // throws -> middleware handles
            return Ok(ApiResponse<HolidayDto>.Ok(holiday));
        }

        // GET /api/admin/holidays/by-date/{siteId}/{date}
        [HttpGet("by-date/{siteId:int}/{date}")]
        public ActionResult<ApiResponse<HolidayDto>> GetByDate(int siteId, string date)
        {
            var holiday = _holidayservice.GetHolidayByDate(siteId, date);  // throws -> middleware handles
            return Ok(ApiResponse<HolidayDto>.Ok(holiday));
        }

        // POST /api/admin/holidays
        [HttpPost]
        public ActionResult<ApiResponse<HolidayDto>> Create([FromBody] HolidayCreateDto dto)
        {
            if (dto is null)
                return BadRequest(ApiResponse<object>.Fail(new { code = "BAD_REQUEST" }, "Request body is required."));

            var created = _holidayservice.CreateHoliday(dto);    // throws -> middleware handles
            return CreatedAtAction(nameof(Get), new { id = created.HolidayId },
                ApiResponse<HolidayDto>.Ok(created, "Holiday created."));
        }

        // PUT /api/admin/holidays/{id}
        [HttpPut("{id:int}")]
        public ActionResult<ApiResponse<HolidayDto>> Update(int id, [FromBody] HolidayUpdateDto dto)
        {
            if (dto is null)
                return BadRequest(ApiResponse<object>.Fail(new { code = "BAD_REQUEST" }, "Request body is required."));

            var updated = _holidayservice.UpdateHoliday(id, dto);  // throws -> middleware handles
            return Ok(ApiResponse<HolidayDto>.Ok(updated, "Holiday updated."));
        }

        // DELETE /api/admin/holidays/{id}
        [HttpDelete("{id:int}")]
        public ActionResult<ApiResponse<object>> Deactivate(int id)
        {
            _holidayservice.DeactivateHoliday(id);  // throws -> middleware handles
            return Ok(ApiResponse<object>.Ok(new { id }, "Holiday deactivated."));
        }

        // POST /api/admin/holidays/{id}/activate
        [HttpPost("{id:int}/activate")]
        public ActionResult<ApiResponse<object>> Activate(int id)
        {
            _holidayservice.ActivateHoliday(id);    // throws -> middleware handles
            return Ok(ApiResponse<object>.Ok(new { id }, "Holiday activated."));
        }
    }
}