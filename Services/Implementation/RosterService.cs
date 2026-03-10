using System;
using System.Collections.Generic;
using CareSchedule.DTOs;
using CareSchedule.Infrastructure;
using CareSchedule.Repositories.Interface;
using CareSchedule.Services.Interface;

namespace CareSchedule.Services.Implementation
{
    public class RosterService : IRosterService
    {
        private readonly IShiftTemplateRepository _shiftRepo;
        private readonly IRosterRepository _rosterRepo;
        private readonly IRosterAssignmentRepository _assignRepo;
        private readonly IOnCallCoverageRepository _onCallRepo;
        private readonly INotificationRepository _notifRepo;
        private readonly IAuditLogRepository _auditRepo;
        private readonly IUnitOfWork _uow;

        public RosterService(
            IShiftTemplateRepository shiftRepo,
            IRosterRepository rosterRepo,
            IRosterAssignmentRepository assignRepo,
            IOnCallCoverageRepository onCallRepo,
            INotificationRepository notifRepo,
            IAuditLogRepository auditRepo,
            IUnitOfWork uow)
        {
            _shiftRepo = shiftRepo;
            _rosterRepo = rosterRepo;
            _assignRepo = assignRepo;
            _onCallRepo = onCallRepo;
            _notifRepo = notifRepo;
            _auditRepo = auditRepo;
            _uow = uow;
        }

        // --------- Shift Templates ---------

        public ShiftTemplateResponseDto CreateShiftTemplate(CreateShiftTemplateDto dto)
        {
            throw new NotImplementedException();
        }

        public ShiftTemplateResponseDto UpdateShiftTemplate(int id, UpdateShiftTemplateDto dto)
        {
            throw new NotImplementedException();
        }

        public void DeleteShiftTemplate(int id)
        {
            throw new NotImplementedException();
        }

        // --------- Rosters ---------

        public RosterResponseDto CreateRoster(CreateRosterDto dto)
        {
            throw new NotImplementedException();
        }

        public RosterResponseDto PublishRoster(int rosterId, PublishRosterDto dto)
        {
            throw new NotImplementedException();
        }

        // --------- Assignments ---------

        public RosterAssignmentResponseDto AssignStaff(CreateRosterAssignmentDto dto)
        {
            throw new NotImplementedException();
        }

        public RosterAssignmentResponseDto SwapShift(int assignmentId, SwapAssignmentDto dto)
        {
            throw new NotImplementedException();
        }

        public void MarkAbsent(int assignmentId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RosterAssignmentResponseDto> SearchAssignments(RosterAssignmentSearchDto dto)
        {
            throw new NotImplementedException();
        }

        // --------- On-Call ---------

        public OnCallResponseDto CreateOnCall(CreateOnCallDto dto)
        {
            throw new NotImplementedException();
        }

        public OnCallResponseDto UpdateOnCall(int id, UpdateOnCallDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
