using CareSchedule.DTOs;

namespace CareSchedule.Services.Interface
{
    public interface IRulesService
    {
        CapacityRuleResponseDto CreateCapacityRule(CreateCapacityRuleDto dto);
        CapacityRuleResponseDto UpdateCapacityRule(int ruleId, UpdateCapacityRuleDto dto);
        SlaResponseDto CreateSla(CreateSlaDto dto);
        SlaResponseDto UpdateSla(int slaId, UpdateSlaDto dto);
    }
}
