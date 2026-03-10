using System;
using System.Collections.Generic;
using CareSchedule.Models;
using CareSchedule.Infrastructure.Data;
using CareSchedule.Repositories.Interface;

namespace CareSchedule.Repositories.Implementation
{
    public class ResourceHoldRepository : IResourceHoldRepository
    {
        private readonly CareScheduleContext _db;

        public ResourceHoldRepository(CareScheduleContext db)
        {
            _db = db;
        }

        public void Add(ResourceHold entity)
        {
            throw new NotImplementedException();
        }

        public void Update(ResourceHold entity)
        {
            throw new NotImplementedException();
        }

        public ResourceHold? GetById(int holdId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ResourceHold> Search(int? siteId, string? resourceType, int? resourceId)
        {
            throw new NotImplementedException();
        }
    }
}
