using System.Collections.Generic;
using CareSchedule.DTOs;

namespace CareSchedule.Services.Interface
{
    public interface IReportService
    {
        IEnumerable<OpsReportResponseDto> Search(ReportSearchDto dto);
        byte[] Export(ReportSearchDto dto);
    }
}
