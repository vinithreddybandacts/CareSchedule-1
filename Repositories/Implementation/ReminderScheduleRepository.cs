using CareSchedule.Models;
using CareSchedule.Infrastructure.Data;
using CareSchedule.Repositories.Interface;

namespace CareSchedule.Repositories.Implementation
{
    public class ReminderScheduleRepository(CareScheduleContext _db) : IReminderScheduleRepository
    {
        public void Add(ReminderSchedule entity) => _db.ReminderSchedules.Add(entity);
    }
}