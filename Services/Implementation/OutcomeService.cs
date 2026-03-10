using System;
using CareSchedule.DTOs;
using CareSchedule.Infrastructure;
using CareSchedule.Repositories.Interface;
using CareSchedule.Services.Interface;

namespace CareSchedule.Services.Implementation
{
    public class OutcomeService : IOutcomeService
    {
        private readonly IOutcomeRepository _outcomeRepo;
        private readonly IAppointmentRepository _apptRepo;
        private readonly IPublishedSlotBookingRepository _slotRepo;
        private readonly IChargeRefRepository _chargeRepo;
        private readonly INotificationRepository _notifRepo;
        private readonly IAuditLogRepository _auditRepo;
        private readonly IUnitOfWork _uow;

        public OutcomeService(
            IOutcomeRepository outcomeRepo,
            IAppointmentRepository apptRepo,
            IPublishedSlotBookingRepository slotRepo,
            IChargeRefRepository chargeRepo,
            INotificationRepository notifRepo,
            IAuditLogRepository auditRepo,
            IUnitOfWork uow)
        {
            _outcomeRepo = outcomeRepo;
            _apptRepo = apptRepo;
            _slotRepo = slotRepo;
            _chargeRepo = chargeRepo;
            _notifRepo = notifRepo;
            _auditRepo = auditRepo;
            _uow = uow;
        }

        public OutcomeResponseDto RecordOutcome(int appointmentId, RecordOutcomeRequestDto dto)
        {
            throw new NotImplementedException();
        }

        public OutcomeResponseDto MarkNoShow(int appointmentId, RecordOutcomeRequestDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
