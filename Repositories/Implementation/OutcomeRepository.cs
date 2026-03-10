using System;
using System.Collections.Generic;
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
            throw new NotImplementedException();
        }

        public Outcome? GetByAppointmentId(int appointmentId)
        {
            throw new NotImplementedException();
        }
    }
}
