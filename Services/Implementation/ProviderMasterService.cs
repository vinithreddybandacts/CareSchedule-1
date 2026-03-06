using CareSchedule.DTOs;
using CareSchedule.Models;
using CareSchedule.Repositories.Interface;
using CareSchedule.Services.Interface;

namespace CareSchedule.Services.Implementation
{
    public class ProviderMasterService : IProviderMasterService
    {
        private readonly IProviderRepository _repo;

        public ProviderMasterService(IProviderRepository repo)
        {
            _repo = repo;
        }

        public List<ProviderDto> GetAllProviders()
        {
            var providers = _repo.GetAll();
            return providers.Select(Map).ToList();
        }

        public ProviderDto? GetProvider(int id)
        {
            var provider = _repo.GetById(id);
            return provider is null ? null : Map(provider);
        }

        public ProviderDto CreateProvider(ProviderCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Provider name is required.");

            var entity = new Provider
            {
                Name = dto.Name.Trim(),
                Specialty = dto.Specialty?.Trim(),
                Credentials = dto.Credentials?.Trim(),
                ContactInfo = dto.ContactInfo?.Trim(),
                Status = "Active"
            };

            entity = _repo.Create(entity);
            return Map(entity);
        }

        public ProviderDto UpdateProvider(int id, ProviderUpdateDto dto)
        {
            var entity = _repo.GetById(id)
                ?? throw new KeyNotFoundException("Provider not found.");

            if (dto.Name is not null)
            {
                if (string.IsNullOrWhiteSpace(dto.Name))
                    throw new ArgumentException("Provider name cannot be empty.");
                entity.Name = dto.Name.Trim();
            }

            if (dto.Specialty is not null)
                entity.Specialty = dto.Specialty.Trim();

            if (dto.Credentials is not null)
                entity.Credentials = dto.Credentials.Trim();

            if (dto.ContactInfo is not null)
                entity.ContactInfo = dto.ContactInfo.Trim();

            _repo.Update(entity);
            return Map(entity);
        }

        public void DeactivateProvider(int id)
        {
            var entity = _repo.GetById(id)
                ?? throw new KeyNotFoundException("Provider not found.");

            if (entity.Status == "Inactive") return;

            entity.Status = "Inactive";
            _repo.Update(entity);
        }

        public void ActivateProvider(int id)
        {
            var entity = _repo.GetById(id)
                ?? throw new KeyNotFoundException("Provider not found.");

            if (entity.Status == "Active") return;

            entity.Status = "Active";
            _repo.Update(entity);
        }

        private static ProviderDto Map(Provider p) => new()
        {
            ProviderId = p.ProviderId,
            Name = p.Name,
            Specialty = p.Specialty,
            Credentials = p.Credentials,
            ContactInfo = p.ContactInfo,
            Status = p.Status
        };
    }
}
