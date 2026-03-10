using System;
using System.Collections.Generic;
using CareSchedule.Models;
using CareSchedule.Infrastructure.Data;
using CareSchedule.Repositories.Interface;

namespace CareSchedule.Repositories.Implementation
{
    public class LeaveImpactRepository : ILeaveImpactRepository
    {
        private readonly CareScheduleContext _db;

        public LeaveImpactRepository(CareScheduleContext db)
        {
            _db = db;
        }

        public void Add(LeaveImpact entity)
        {
            throw new NotImplementedException();
        }

        public void Update(LeaveImpact entity)
        {
            throw new NotImplementedException();
        }

        public LeaveImpact? GetById(int impactId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LeaveImpact> GetByLeaveId(int leaveId)
        {
            throw new NotImplementedException();
        }
    }
}
