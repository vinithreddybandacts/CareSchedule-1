using System;
using System.Collections.Generic;
using CareSchedule.Models;
using CareSchedule.Infrastructure.Data;
using CareSchedule.Repositories.Interface;

namespace CareSchedule.Repositories.Implementation
{
    public class OnCallCoverageRepository : IOnCallCoverageRepository
    {
        private readonly CareScheduleContext _db;

        public OnCallCoverageRepository(CareScheduleContext db)
        {
            _db = db;
        }

        public void Add(OnCallCoverage entity)
        {
            throw new NotImplementedException();
        }

        public void Update(OnCallCoverage entity)
        {
            throw new NotImplementedException();
        }

        public OnCallCoverage? GetById(int onCallId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<OnCallCoverage> Search(int? siteId, DateOnly? date)
        {
            throw new NotImplementedException();
        }
    }
}
