using System;
using System.Collections.Generic;
using System.Linq;
using CareSchedule.Models;
using CareSchedule.Infrastructure.Data;
using CareSchedule.Repositories.Interface;

namespace CareSchedule.Repositories.Implementation
{
    public class OutcomeRepository : IOutcomeRepository
    {
        private readonly CareScheduleContext _db;

        public OutcomeRepository(CareScheduleContext db)
        {
            _db = db;
        }

        public void Add(Outcome entity)
        {
            _db.Outcomes.Add(entity);
            _db.SaveChanges();
        }

        public Outcome? GetByAppointmentId(int appointmentId)
        {
            return _db.Outcomes.FirstOrDefault(o => o.AppointmentId == appointmentId);
        }
    }
}