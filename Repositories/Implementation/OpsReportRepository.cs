using System;
using System.Collections.Generic;
using CareSchedule.Models;
using CareSchedule.Infrastructure.Data;
using CareSchedule.Repositories.Interface;

namespace CareSchedule.Repositories.Implementation
{
    public class OpsReportRepository : IOpsReportRepository
    {
        private readonly CareScheduleContext _db;

        public OpsReportRepository(CareScheduleContext db)
        {
            _db = db;
        }

        public void Add(OpsReport entity)
        {
            throw new NotImplementedException();
        }

        public OpsReport? GetById(int reportId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<OpsReport> Search(string? scope, DateTime? from, DateTime? to)
        {
            throw new NotImplementedException();
        }
    }
}
