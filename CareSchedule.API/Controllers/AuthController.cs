using Microsoft.AspNetCore.Mvc;
using CareSchedule.API.Contracts;
using CareSchedule.Services.Interface;
using CareSchedule.DTOs;

namespace CareSchedule.API.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController(IAuthService _authService) : ControllerBase
    {
        // POST /auth/login
        [HttpPost("login")]
        public ActionResult<ApiResponse<LoginResponseDto>> Login([FromBody] LoginRequestDto dto)
        {
            var result = _authService.Login(dto.Email, dto.Role);
            return ApiResponse<LoginResponseDto>.Ok(result, "Login successful.");
        }

        // POST /auth/logout
        [HttpPost("logout")]
        public ActionResult<ApiResponse<object>> Logout([FromBody] LogoutRequestDto dto)
        {
            _authService.Logout(dto.UserId);
            return ApiResponse<object>.Ok(null, "Logout successful.");
        }
    }
}