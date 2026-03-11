using System;
using System.Collections.Generic;
using System.Linq;
using CareSchedule.Models;
using CareSchedule.Infrastructure.Data;
using CareSchedule.Repositories.Interface;

namespace CareSchedule.Repositories.Implementation
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly CareScheduleContext _db;
        public NotificationRepository(CareScheduleContext db) { _db = db; }

        public void Add(Notification entity)
        {
            _db.Notifications.Add(entity);
            _db.SaveChanges();
        }

        public Notification? GetById(int notificationId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Notification> GetByUserId(int userId)
        {
            throw new NotImplementedException();
        }

        public void Update(Notification entity)
        {
            throw new NotImplementedException();
        }

    }
}