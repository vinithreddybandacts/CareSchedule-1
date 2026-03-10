using Microsoft.AspNetCore.Mvc;
using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("shift-templates")]
    public class ShiftTemplatesController : ControllerBase
    {
        private readonly IRosterService _service;

        public ShiftTemplatesController(IRosterService service)
        {
            _service = service;
        }

        [HttpPost]
        public ActionResult<ApiResponse<ShiftTemplateResponseDto>> Create([FromBody] CreateShiftTemplateDto dto)
        {
            var result = _service.CreateShiftTemplate(dto);
            return ApiResponse<ShiftTemplateResponseDto>.Ok(result, "Shift template created.");
        }

        [HttpPut("{id:int}")]
        public ActionResult<ApiResponse<ShiftTemplateResponseDto>> Update(int id, [FromBody] UpdateShiftTemplateDto dto)
        {
            var result = _service.UpdateShiftTemplate(id, dto);
            return ApiResponse<ShiftTemplateResponseDto>.Ok(result, "Shift template updated.");
        }

        [HttpDelete("{id:int}")]
        public ActionResult<ApiResponse<object>> Delete(int id)
        {
            _service.DeleteShiftTemplate(id);
            return ApiResponse<object>.Ok(null, "Shift template deleted.");
        }
    }
}
