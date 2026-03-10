using Microsoft.AspNetCore.Mvc;
using CareSchedule.API.Contracts;
using CareSchedule.DTOs;
using CareSchedule.Services.Interface;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("charges")]
    public class ChargesController : ControllerBase
    {
        private readonly IBillingService _service;

        public ChargesController(IBillingService service)
        {
            _service = service;
        }

        [HttpPost]
        public ActionResult<ApiResponse<ChargeRefResponseDto>> Create([FromBody] CreateChargeRefDto dto)
        {
            var result = _service.CreateCharge(dto);
            return ApiResponse<ChargeRefResponseDto>.Ok(result, "Charge created.");
        }
    }
}
