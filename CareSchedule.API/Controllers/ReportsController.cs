using Microsoft.AspNetCore.Mvc;
using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("reports")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _service;

        public ReportsController(IReportService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<ApiResponse<IEnumerable<OpsReportResponseDto>>> Search([FromQuery] ReportSearchDto dto)
        {
            var list = _service.Search(dto);
            return ApiResponse<IEnumerable<OpsReportResponseDto>>.Ok(list, "Reports fetched.");
        }

        [HttpGet("export")]
        public IActionResult Export([FromQuery] ReportSearchDto dto)
        {
            var bytes = _service.Export(dto);
            return File(bytes, "application/octet-stream", "report.csv");
        }
    }
}
