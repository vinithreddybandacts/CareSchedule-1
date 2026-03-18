using System;
using System.Collections.Generic;
using CareSchedule.DTOs;
using CareSchedule.Repositories.Interface;
using CareSchedule.Services.Interface;

namespace CareSchedule.Services.Implementation
{
    public class ReportService(IOpsReportRepository _reportRepo) : IReportService
    {
        public IEnumerable<OpsReportResponseDto> Search(ReportSearchDto dto)
        {
            throw new NotImplementedException();
        }

        public byte[] Export(ReportSearchDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
