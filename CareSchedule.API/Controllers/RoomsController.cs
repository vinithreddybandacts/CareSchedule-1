using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using CareSchedule.Shared.Exceptions;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("api/masterdata/rooms")]
    [Produces("application/json")]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _service;

        public RoomsController(IRoomService service)
        {
            _service = service;
        }

        // GET /api/masterdata/rooms
        [HttpGet]
        public IActionResult Search([FromQuery] RoomSearchQuery query)
        {
            var items = _service.SearchRoom(query);
            return Ok(ApiResponse<object>.Ok(items));
        }

        // GET /api/masterdata/rooms/{id}
        [HttpGet("{id:int}")]
        public ActionResult<ApiResponse<RoomDto>> Get(int id)
        {
            var room = _service.GetRoom(id);
            return room is null
                ? NotFound(ApiResponse<object>.Fail(new { code = "RESOURCE_NOT_FOUND" }, "Room not found."))
                : Ok(ApiResponse<RoomDto>.Ok(room));
        }

        // POST /api/masterdata/rooms
        [HttpPost]
        // POST /api/masterdata/rooms
        [HttpPost]
        public ActionResult<ApiResponse<RoomDto>> Create([FromBody] RoomCreateDto dto)
        {
            if (dto is null)
                return BadRequest(ApiResponse<object>.Fail(new { code = "BAD_REQUEST" }, "Request body is required."));

            try
            {
                var created = _service.CreateRoom(dto);
                return CreatedAtAction(nameof(Get), new { id = created.RoomId },
                    ApiResponse<RoomDto>.Ok(created, "Room created."));
            }
            catch (SiteNotFoundForRoomException)
            {
                return NotFound(ApiResponse<object>.Fail(new { code = "RESOURCE_NOT_FOUND" }, "Site not found."));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(new { code = "BAD_REQUEST" }, ex.Message));
            }
        }

        // PUT /api/masterdata/rooms/{id}
        [HttpPut("{id:int}")]
        public ActionResult<ApiResponse<RoomDto>> Update(int id, [FromBody] RoomUpdateDto dto)
        {
            if (dto is null)
                return BadRequest(ApiResponse<object>.Fail(new { code = "BAD_REQUEST" }, "Request body is required."));

            try
            {
                var updated = _service.UpdateRoom(id, dto);
                return Ok(ApiResponse<RoomDto>.Ok(updated, "Room updated."));
            }
            catch (RoomNotFoundException)
            {
                return NotFound(ApiResponse<object>.Fail(new { code = "RESOURCE_NOT_FOUND" }, "Room not found."));
            }
            catch (SiteNotFoundForRoomException)
            {
                return NotFound(ApiResponse<object>.Fail(new { code = "RESOURCE_NOT_FOUND" }, "Site not found."));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(new { code = "BAD_REQUEST" }, ex.Message));
            }
        }

        // DELETE /api/masterdata/rooms/{id}  (soft delete -> Status = Inactive)
        [HttpDelete("{id:int}")]
        public ActionResult<ApiResponse<object>> Deactivate(int id)
        {
            return HandleStatusChange(id, _service.DeactivateRoom, "Room deactivated.");
        }

        // POST /api/masterdata/rooms/{id}/activate
        [HttpPost("{id:int}/activate")]
        public ActionResult<ApiResponse<object>> Activate(int id)
        {
            return HandleStatusChange(id, _service.ActivateRoom, "Room activated.");
        }

        // Helper
        private ActionResult<ApiResponse<object>> HandleStatusChange(int id, Action<int> action, string message)
        {
            try
            {
                action(id);
                return Ok(ApiResponse<object>.Ok(new { id }, message));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse<object>.Fail(new { code = "RESOURCE_NOT_FOUND" }, "Room not found."));
            }
        }
    }
}