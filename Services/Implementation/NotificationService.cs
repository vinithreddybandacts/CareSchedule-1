using System;
using System.Collections.Generic;
using System.Linq;
using CareSchedule.DTOs;
using CareSchedule.Infrastructure;
using CareSchedule.Models;
using CareSchedule.Repositories.Interface;
using CareSchedule.Services.Interface;

namespace CareSchedule.Services.Implementation
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notifRepo;
        private readonly IUnitOfWork _uow;

        public NotificationService(
            INotificationRepository notifRepo,
            IUnitOfWork uow)
        {
            _notifRepo = notifRepo;
            _uow = uow;
        }

        public IEnumerable<NotificationResponseDto> GetByUserId(int userId)
        {
            if (userId <= 0) throw new ArgumentException("Invalid userId.");
            var items = _notifRepo.GetByUserId(userId);
            return items.Select(Map).ToList();
        }

        public void MarkAsRead(int notificationId)
        {
            var entity = _notifRepo.GetById(notificationId);
            if (entity == null) throw new KeyNotFoundException($"Notification {notificationId} not found.");
            entity.Status = "Read";
            _notifRepo.Update(entity);
        }

        public void Dismiss(int notificationId)
        {
            var entity = _notifRepo.GetById(notificationId);
            if (entity == null) throw new KeyNotFoundException($"Notification {notificationId} not found.");
            entity.Status = "Dismissed";
            _notifRepo.Update(entity);
        }

        public void Create(int userId, string message, string category)
        {
            var entity = new Notification
            {
                UserId = userId,
                Message = message,
                Category = category,
                Status = "Unread",
                CreatedDate = DateTime.UtcNow
            };
            _notifRepo.Add(entity);
            _uow.SaveChanges();
        }

        private static NotificationResponseDto Map(Notification n) => new()
        {
            NotificationId = n.NotificationId,
            UserId = n.UserId,
            Message = n.Message,
            Category = n.Category,
            Status = n.Status,
            CreatedDate = n.CreatedDate
        };
    }
}