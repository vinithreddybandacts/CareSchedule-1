using Microsoft.AspNetCore.Mvc;
using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("slots")]
    public class SlotsController(IAvailabilityService _availabilityservice) : ControllerBase
    {
        // GET /slots?providerId=&serviceId=&siteId=&date=YYYY-MM-DD
        [HttpGet]
        public ActionResult<ApiResponse<IEnumerable<SlotResponseDto>>> Get([FromQuery] int providerId, [FromQuery] int serviceId, [FromQuery] int siteId, [FromQuery] string date)
        {
            var data = _availabilityservice.GetOpenSlots(new SlotSearchRequestDto
            {
                ProviderId = providerId,
                ServiceId = serviceId,
                SiteId = siteId,
                Date = date
            });
            return ApiResponse<IEnumerable<SlotResponseDto>>.Ok(data, "Slots fetched.");
        }
    }
}