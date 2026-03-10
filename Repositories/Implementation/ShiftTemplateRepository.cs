using System;
using System.Collections.Generic;
using CareSchedule.Models;
using CareSchedule.Infrastructure.Data;
using CareSchedule.Repositories.Interface;

namespace CareSchedule.Repositories.Implementation
{
    public class ShiftTemplateRepository : IShiftTemplateRepository
    {
        private readonly CareScheduleContext _db;

        public ShiftTemplateRepository(CareScheduleContext db)
        {
            _db = db;
        }

        public void Add(ShiftTemplate entity)
        {
            throw new NotImplementedException();
        }

        public void Update(ShiftTemplate entity)
        {
            throw new NotImplementedException();
        }

        public ShiftTemplate? GetById(int shiftTemplateId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ShiftTemplate> Search(int? siteId, string? role, string? status)
        {
            throw new NotImplementedException();
        }
    }
}
