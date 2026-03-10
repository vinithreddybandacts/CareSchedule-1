using System;
using System.Collections.Generic;
using CareSchedule.Models;
using CareSchedule.Infrastructure.Data;
using CareSchedule.Repositories.Interface;

namespace CareSchedule.Repositories.Implementation
{
    public class CheckInRepository : ICheckInRepository
    {
        private readonly CareScheduleContext _db;

        public CheckInRepository(CareScheduleContext db)
        {
            _db = db;
        }

        public void Add(CheckIn entity)
        {
            throw new NotImplementedException();
        }

        public void Update(CheckIn entity)
        {
            throw new NotImplementedException();
        }

        public CheckIn? GetById(int checkInId)
        {
            throw new NotImplementedException();
        }

        public CheckIn? GetByAppointmentId(int appointmentId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CheckIn> Search(int? siteId, int? providerId, int? nurseId, string? status)
        {
            throw new NotImplementedException();
        }
    }
}
