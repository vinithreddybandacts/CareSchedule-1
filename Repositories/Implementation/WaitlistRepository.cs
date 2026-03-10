using System;
using System.Collections.Generic;
using CareSchedule.Models;
using CareSchedule.Infrastructure.Data;
using CareSchedule.Repositories.Interface;

namespace CareSchedule.Repositories.Implementation
{
    public class WaitlistRepository : IWaitlistRepository
    {
        private readonly CareScheduleContext _db;

        public WaitlistRepository(CareScheduleContext db)
        {
            _db = db;
        }

        public void Add(Waitlist entity)
        {
            throw new NotImplementedException();
        }

        public void Update(Waitlist entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Waitlist entity)
        {
            throw new NotImplementedException();
        }

        public Waitlist? GetById(int waitId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Waitlist> Search(int? siteId, int? providerId, int? serviceId, int? patientId, string? status)
        {
            throw new NotImplementedException();
        }
    }
}
