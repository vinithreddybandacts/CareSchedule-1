using CareSchedule.Models;
using CareSchedule.Repositories.Interface;
using CareSchedule.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CareSchedule.Repositories.Implementation
{
    public class ProviderRepository : IProviderRepository
    {
        private readonly CareScheduleContext _db;

        public ProviderRepository(CareScheduleContext db)
        {
            _db = db;
        }

        public List<Provider> GetAll()
        {
            return _db.Providers.AsNoTracking().ToList();
        }

        public Provider? GetById(int id)
        {
            return _db.Providers.AsNoTracking().FirstOrDefault(p => p.ProviderId == id);
        }

        public Provider Create(Provider entity)
        {
            _db.Providers.Add(entity);
            _db.SaveChanges();
            return entity;
        }

        public void Update(Provider entity)
        {
            _db.Providers.Update(entity);
            _db.SaveChanges();
        }
    }
}
