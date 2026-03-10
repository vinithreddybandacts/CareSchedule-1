using System;
using System.Collections.Generic;
using CareSchedule.Models;
using CareSchedule.Infrastructure.Data;
using CareSchedule.Repositories.Interface;

namespace CareSchedule.Repositories.Implementation
{
    public class CapacityRuleRepository : ICapacityRuleRepository
    {
        private readonly CareScheduleContext _db;

        public CapacityRuleRepository(CareScheduleContext db)
        {
            _db = db;
        }

        public void Add(CapacityRule entity)
        {
            throw new NotImplementedException();
        }

        public void Update(CapacityRule entity)
        {
            throw new NotImplementedException();
        }

        public CapacityRule? GetById(int ruleId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CapacityRule> Search(string? scope, string? status)
        {
            throw new NotImplementedException();
        }
    }
}
