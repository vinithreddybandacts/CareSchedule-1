using System.Collections.Generic;
using CareSchedule.DTOs;

namespace CareSchedule.Services.Interface
{
    public interface IRosterService
    {
        // Shift Templates
        ShiftTemplateResponseDto CreateShiftTemplate(CreateShiftTemplateDto dto);
        ShiftTemplateResponseDto UpdateShiftTemplate(int id, UpdateShiftTemplateDto dto);
        void DeleteShiftTemplate(int id);

        // Rosters
        RosterResponseDto CreateRoster(CreateRosterDto dto);
        RosterResponseDto PublishRoster(int rosterId, PublishRosterDto dto);

        // Assignments
        RosterAssignmentResponseDto AssignStaff(CreateRosterAssignmentDto dto);
        RosterAssignmentResponseDto SwapShift(int assignmentId, SwapAssignmentDto dto);
        void MarkAbsent(int assignmentId);
        IEnumerable<RosterAssignmentResponseDto> SearchAssignments(RosterAssignmentSearchDto dto);

        // On-Call
        OnCallResponseDto CreateOnCall(CreateOnCallDto dto);
        OnCallResponseDto UpdateOnCall(int id, UpdateOnCallDto dto);
    }
}
