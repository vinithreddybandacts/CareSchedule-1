using System;
using System.Collections.Generic;
using CareSchedule.Models;
using CareSchedule.Infrastructure.Data;
using CareSchedule.Repositories.Interface;

namespace CareSchedule.Repositories.Implementation
{
    public class RosterAssignmentRepository : IRosterAssignmentRepository
    {
        private readonly CareScheduleContext _db;

        public RosterAssignmentRepository(CareScheduleContext db)
        {
            _db = db;
        }

        public void Add(RosterAssignment entity)
        {
            throw new NotImplementedException();
        }

        public void Update(RosterAssignment entity)
        {
            throw new NotImplementedException();
        }

        public RosterAssignment? GetById(int assignmentId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RosterAssignment> Search(int? siteId, int? userId, DateOnly? date, string? status)
        {
            throw new NotImplementedException();
        }
    }
}
