using System;
using System.Collections.Generic;
using CareSchedule.Models;
using CareSchedule.Infrastructure.Data;
using CareSchedule.Repositories.Interface;

namespace CareSchedule.Repositories.Implementation
{
    public class LeaveRequestRepository(CareScheduleContext _db) : ILeaveRequestRepository
    {
        public void Add(LeaveRequest entity)
        {
            throw new NotImplementedException();
        }

        public void Update(LeaveRequest entity)
        {
            throw new NotImplementedException();
        }

        public LeaveRequest? GetById(int leaveId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LeaveRequest> Search(int? userId, string? status)
        {
            throw new NotImplementedException();
        }
    }
}
