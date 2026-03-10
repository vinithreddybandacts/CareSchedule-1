using Microsoft.AspNetCore.Mvc;
using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("notifications")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _service;

        public NotificationsController(INotificationService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<ApiResponse<IEnumerable<NotificationResponseDto>>> GetByUser([FromQuery] int userId)
        {
            var list = _service.GetByUserId(userId);
            return ApiResponse<IEnumerable<NotificationResponseDto>>.Ok(list, "Notifications fetched.");
        }

        [HttpPatch("{notificationId:int}/read")]
        public ActionResult<ApiResponse<object>> MarkAsRead(int notificationId)
        {
            _service.MarkAsRead(notificationId);
            return ApiResponse<object>.Ok(new { notificationId }, "Notification marked as read.");
        }

        [HttpPatch("{notificationId:int}/dismiss")]
        public ActionResult<ApiResponse<object>> Dismiss(int notificationId)
        {
            _service.Dismiss(notificationId);
            return ApiResponse<object>.Ok(new { notificationId }, "Notification dismissed.");
        }
    }
}
