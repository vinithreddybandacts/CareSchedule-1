using System;
using System.Collections.Generic;
using CareSchedule.Models;
using CareSchedule.Infrastructure.Data;
using CareSchedule.Repositories.Interface;

namespace CareSchedule.Repositories.Implementation
{
    public class RosterRepository : IRosterRepository
    {
        private readonly CareScheduleContext _db;

        public RosterRepository(CareScheduleContext db)
        {
            _db = db;
        }

        public void Add(Roster entity)
        {
            throw new NotImplementedException();
        }

        public void Update(Roster entity)
        {
            throw new NotImplementedException();
        }

        public Roster? GetById(int rosterId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Roster> Search(int? siteId, string? status)
        {
            throw new NotImplementedException();
        }
    }
}
