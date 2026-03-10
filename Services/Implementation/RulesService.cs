using System;
using CareSchedule.DTOs;
using CareSchedule.Infrastructure;
using CareSchedule.Repositories.Interface;
using CareSchedule.Services.Interface;

namespace CareSchedule.Services.Implementation
{
    public class RulesService : IRulesService
    {
        private readonly ICapacityRuleRepository _capacityRepo;
        private readonly ISlaRepository _slaRepo;
        private readonly IAuditLogRepository _auditRepo;
        private readonly IUnitOfWork _uow;

        public RulesService(
            ICapacityRuleRepository capacityRepo,
            ISlaRepository slaRepo,
            IAuditLogRepository auditRepo,
            IUnitOfWork uow)
        {
            _capacityRepo = capacityRepo;
            _slaRepo = slaRepo;
            _auditRepo = auditRepo;
            _uow = uow;
        }

        public CapacityRuleResponseDto CreateCapacityRule(CreateCapacityRuleDto dto)
        {
            throw new NotImplementedException();
        }

        public CapacityRuleResponseDto UpdateCapacityRule(int ruleId, UpdateCapacityRuleDto dto)
        {
            throw new NotImplementedException();
        }

        public SlaResponseDto CreateSla(CreateSlaDto dto)
        {
            throw new NotImplementedException();
        }

        public SlaResponseDto UpdateSla(int slaId, UpdateSlaDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
