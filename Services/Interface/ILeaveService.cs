using System.Collections.Generic;
using CareSchedule.DTOs;

namespace CareSchedule.Services.Interface
{
    public interface ILeaveService
    {
        IEnumerable<LeaveRequestResponseDto> Search(LeaveSearchDto dto);
        LeaveRequestResponseDto Approve(int leaveId);
        LeaveRequestResponseDto Reject(int leaveId);
        LeaveImpactResponseDto CreateImpact(CreateLeaveImpactDto dto);
        LeaveImpactResponseDto ResolveImpact(int impactId, ResolveLeaveImpactDto dto);
    }
}
