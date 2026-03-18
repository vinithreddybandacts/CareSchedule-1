using System;
using System.Collections.Generic;
using CareSchedule.DTOs;
using CareSchedule.Infrastructure;
using CareSchedule.Repositories.Interface;
using CareSchedule.Services.Interface;

namespace CareSchedule.Services.Implementation
{
    public class LeaveService(
            ILeaveRequestRepository _leaveRepo,
            ILeaveImpactRepository _impactRepo,
            IAppointmentRepository _apptRepo,
            IRosterAssignmentRepository _rosterAssignRepo,
            INotificationRepository _notifRepo,
            IAuditLogService _auditService,
            IAvailabilityBlockRepository _blockRepo,
            IUnitOfWork _uow) : ILeaveService
    {
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