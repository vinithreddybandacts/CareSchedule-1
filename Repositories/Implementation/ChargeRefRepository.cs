using System;
using System.Collections.Generic;
using CareSchedule.Models;
using CareSchedule.Infrastructure.Data;
using CareSchedule.Repositories.Interface;

namespace CareSchedule.Repositories.Implementation
{
    public class ChargeRefRepository(CareScheduleContext _db) : IChargeRefRepository
    {
        public void Add(ChargeRef entity)
        {
            throw new NotImplementedException();
        }

        public ChargeRef? GetByAppointmentId(int appointmentId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ChargeRef> Search(int? appointmentId, int? providerId, string? status)
        {
            throw new NotImplementedException();
        }
    }
}
