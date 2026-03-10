using System;
using System.Collections.Generic;
using CareSchedule.DTOs;
using CareSchedule.Infrastructure;
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
            throw new NotImplementedException();
        }

        public void MarkAsRead(int notificationId)
        {
            throw new NotImplementedException();
        }

        public void Dismiss(int notificationId)
        {
            throw new NotImplementedException();
        }

        public void Create(int userId, string message, string category)
        {
            throw new NotImplementedException();
        }
    }
}
