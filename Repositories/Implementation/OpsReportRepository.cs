using System;
using System.Collections.Generic;
using CareSchedule.Models;
using CareSchedule.Infrastructure.Data;
using CareSchedule.Repositories.Interface;

namespace CareSchedule.Repositories.Implementation
{
    public class OpsReportRepository(CareScheduleContext _db) : IOpsReportRepository
    {
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
