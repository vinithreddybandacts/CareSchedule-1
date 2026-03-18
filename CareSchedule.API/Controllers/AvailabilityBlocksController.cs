using Microsoft.AspNetCore.Mvc;
using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("availability-blocks")]
    public class AvailabilityBlocksController(IAvailabilityService _availabilityservice) : ControllerBase
    {
        // POST /availability-blocks
        [HttpPost]
        public ActionResult<ApiResponse<IdResponseDto>> Create([FromBody] CreateAvailabilityBlockRequestDto dto)
        {
            var id = _availabilityservice.CreateBlock(dto);
            return ApiResponse<IdResponseDto>.Ok(new IdResponseDto { Id = id }, "Block created.");
        }

        // DELETE /availability-blocks/{blockId}
        [HttpDelete("{blockId:int}")]
        public ActionResult<ApiResponse<object>> Delete(int blockId)
        {
            _availabilityservice.RemoveBlock(blockId);
            return ApiResponse<object>.Ok(null, "Block removed.");
        }

        // GET /availability-blocks?providerId=&siteId=&date=
        [HttpGet]
        public ActionResult<ApiResponse<IEnumerable<AvailabilityBlockResponseDto>>> List([FromQuery] int providerId, [FromQuery] int siteId, [FromQuery] string? date)
        {
            var data = _availabilityservice.ListBlocks(providerId, siteId, date);
            return ApiResponse<IEnumerable<AvailabilityBlockResponseDto>>.Ok(data, "Blocks fetched.");
        }
    }
}