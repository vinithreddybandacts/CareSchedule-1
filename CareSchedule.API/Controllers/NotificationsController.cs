using Microsoft.AspNetCore.Mvc;
using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("notifications")]
    public class NotificationsController(INotificationService _notificationservice) : ControllerBase
    {
        [HttpGet]
        public ActionResult<ApiResponse<IEnumerable<NotificationResponseDto>>> GetByUser([FromQuery] int userId)
        {
            var list = _notificationservice.GetByUserId(userId);
            return ApiResponse<IEnumerable<NotificationResponseDto>>.Ok(list, "Notifications fetched.");
        }

        [HttpPatch("{notificationId:int}/read")]
        public ActionResult<ApiResponse<object>> MarkAsRead(int notificationId)
        {
            _notificationservice.MarkAsRead(notificationId);
            return ApiResponse<object>.Ok(new { notificationId }, "Notification marked as read.");
        }

        [HttpPatch("{notificationId:int}/dismiss")]
        public ActionResult<ApiResponse<object>> Dismiss(int notificationId)
        {
            _notificationservice.Dismiss(notificationId);
            return ApiResponse<object>.Ok(new { notificationId }, "Notification dismissed.");
        }
    }
}
