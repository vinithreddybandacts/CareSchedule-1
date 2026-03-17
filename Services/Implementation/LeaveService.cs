using System;
using System.Collections.Generic;
using CareSchedule.DTOs;
using CareSchedule.Infrastructure;
using CareSchedule.Repositories.Interface;
using CareSchedule.Services.Interface;

namespace CareSchedule.Services.Implementation
{
    public class LeaveService : ILeaveService
    {
        private readonly ILeaveRequestRepository _leaveRepo;
        private readonly ILeaveImpactRepository _impactRepo;
        private readonly IAppointmentRepository _apptRepo;
        private readonly IRosterAssignmentRepository _rosterAssignRepo;
        private readonly INotificationRepository _notifRepo;
        private readonly IAuditLogService _auditService;
        private readonly IAvailabilityBlockRepository _blockRepo;
        private readonly IUnitOfWork _uow;

        public LeaveService(
            ILeaveRequestRepository leaveRepo,
            ILeaveImpactRepository impactRepo,
            IAppointmentRepository apptRepo,
            IRosterAssignmentRepository rosterAssignRepo,
            INotificationRepository notifRepo,
            IAuditLogService auditService,
            IAvailabilityBlockRepository blockRepo,
            IUnitOfWork uow)
        {
            _leaveRepo = leaveRepo;
            _impactRepo = impactRepo;
            _apptRepo = apptRepo;
            _rosterAssignRepo = rosterAssignRepo;
            _notifRepo = notifRepo;
            _auditService = auditService;
            _blockRepo = blockRepo;
            _uow = uow;
        }

        public IEnumerable<LeaveRequestResponseDto> Search(LeaveSearchDto dto)
        {
            throw new NotImplementedException();
        }

        public LeaveRequestResponseDto Approve(int leaveId)
        {
            throw new NotImplementedException();
        }

        public LeaveRequestResponseDto Reject(int leaveId)
        {
            throw new NotImplementedException();
        }

        public LeaveImpactResponseDto CreateImpact(CreateLeaveImpactDto dto)
        {
            throw new NotImplementedException();
        }

        public LeaveImpactResponseDto ResolveImpact(int impactId, ResolveLeaveImpactDto dto)
        {
            throw new NotImplementedException();
        }
    }
}