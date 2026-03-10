using System;
using System.Collections.Generic;
using CareSchedule.DTOs;
using CareSchedule.Infrastructure;
using CareSchedule.Repositories.Interface;
using CareSchedule.Services.Interface;

namespace CareSchedule.Services.Implementation
{
    public class CheckInService : ICheckInService
    {
        private readonly ICheckInRepository _checkInRepo;
        private readonly IAppointmentRepository _apptRepo;
        private readonly INotificationRepository _notifRepo;
        private readonly IAuditLogRepository _auditRepo;
        private readonly IUnitOfWork _uow;

        public CheckInService(
            ICheckInRepository checkInRepo,
            IAppointmentRepository apptRepo,
            INotificationRepository notifRepo,
            IAuditLogRepository auditRepo,
            IUnitOfWork uow)
        {
            _checkInRepo = checkInRepo;
            _apptRepo = apptRepo;
            _notifRepo = notifRepo;
            _auditRepo = auditRepo;
            _uow = uow;
        }

        public CheckInResponseDto CheckIn(int appointmentId, CreateCheckInRequestDto dto)
        {
            throw new NotImplementedException();
        }

        public CheckInResponseDto AssignRoom(int checkInId, AssignRoomRequestDto dto)
        {
            throw new NotImplementedException();
        }

        public CheckInResponseDto MoveToRoom(int checkInId)
        {
            throw new NotImplementedException();
        }

        public CheckInResponseDto UpdateStatus(int checkInId, UpdateCheckInStatusDto dto)
        {
            throw new NotImplementedException();
        }

        public CheckInResponseDto StartConsultation(int checkInId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CheckInResponseDto> Search(CheckInSearchDto dto)
        {
            throw new NotImplementedException();
        }

        public CheckInResponseDto GetById(int checkInId)
        {
            throw new NotImplementedException();
        }
    }
}
