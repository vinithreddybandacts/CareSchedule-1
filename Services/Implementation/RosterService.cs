using System;
using System.Collections.Generic;
using CareSchedule.DTOs;
using CareSchedule.Infrastructure;
using CareSchedule.Repositories.Interface;
using CareSchedule.Services.Interface;

namespace CareSchedule.Services.Implementation
{
    public class RosterService(
            IShiftTemplateRepository _shiftRepo,
            IRosterRepository _rosterRepo,
            IRosterAssignmentRepository _assignRepo,
            IOnCallCoverageRepository _onCallRepo,
            INotificationRepository _notifRepo,
            IAuditLogService _auditService,
            IUnitOfWork _uow) : IRosterService
    {
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