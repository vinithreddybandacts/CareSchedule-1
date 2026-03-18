using System;
using System.Collections.Generic;
using CareSchedule.Models;
using CareSchedule.Infrastructure.Data;
using CareSchedule.Repositories.Interface;

namespace CareSchedule.Repositories.Implementation
{
    public class LeaveImpactRepository(CareScheduleContext _db) : ILeaveImpactRepository
    {
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
