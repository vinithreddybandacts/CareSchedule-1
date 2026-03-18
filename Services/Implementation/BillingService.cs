using System;
using CareSchedule.DTOs;
using CareSchedule.Infrastructure;
using CareSchedule.Repositories.Interface;
using CareSchedule.Services.Interface;

namespace CareSchedule.Services.Implementation
{
    public class BillingService(
            IChargeRefRepository _chargeRepo,
            IAuditLogService _auditService,
            IUnitOfWork _uow)
            : IBillingService
    {
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