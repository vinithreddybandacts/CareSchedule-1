using System;
using System.Collections.Generic;
using CareSchedule.Models;
using CareSchedule.Infrastructure.Data;
using CareSchedule.Repositories.Interface;

namespace CareSchedule.Repositories.Implementation
{
    public class SlaRepository : ISlaRepository
    {
        private readonly CareScheduleContext _db;

        public SlaRepository(CareScheduleContext db)
        {
            _db = db;
        }

        public void Add(Sla entity)
        {
            throw new NotImplementedException();
        }

        public void Update(Sla entity)
        {
            throw new NotImplementedException();
        }

        public Sla? GetById(int slaId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Sla> Search(string? scope, string? status)
        {
            throw new NotImplementedException();
        }
    }
}
