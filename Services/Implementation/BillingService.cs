using System;
using CareSchedule.DTOs;
using CareSchedule.Infrastructure;
using CareSchedule.Repositories.Interface;
using CareSchedule.Services.Interface;

namespace CareSchedule.Services.Implementation
{
    public class BillingService : IBillingService
    {
        private readonly IChargeRefRepository _chargeRepo;
        private readonly IAuditLogRepository _auditRepo;
        private readonly IUnitOfWork _uow;

        public BillingService(
            IChargeRefRepository chargeRepo,
            IAuditLogRepository auditRepo,
            IUnitOfWork uow)
        {
            _chargeRepo = chargeRepo;
            _auditRepo = auditRepo;
            _uow = uow;
        }

        public ChargeRefResponseDto CreateCharge(CreateChargeRefDto dto)
        {
            throw new NotImplementedException();
        }

        public ChargeRefResponseDto GetByAppointment(int appointmentId)
        {
            throw new NotImplementedException();
        }
    }
}
