using Microservices.AuthAPI.Models;
using Microservices.service.AuthAPI.Models.Dto;
using Microservices.service.AuthAPI.Services.Iservices;
using Microsoft.AspNetCore.Mvc;

namespace Microservices.service.AuthAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
        var errorMessage = await authService.RegisterAsync(registerDto);
        if (!string.IsNullOrEmpty(errorMessage))
        {
            ResultDto<string> result = ResultDto<string>.FailureResult(errorMessage);
            return BadRequest(result);
        }
        return Ok(ResultDto<string>.SuccessResult(""));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto model)
    {
        var loginResponse = await authService.LoginAsync(model);
        if (loginResponse.User == null)
        {
            ResultDto<LoginResponseDto> result = ResultDto<LoginResponseDto>.FailureResult("Invalid username or password");
            return BadRequest(result);
        }
        return Ok(ResultDto<LoginResponseDto>.SuccessResult(loginResponse));
    }

    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole(RegisterDto model)
    {
        var isRoleAssigned = await authService.AssignRoleAsync(model.Email, model.Role);
        if (!isRoleAssigned)
        {
            ResultDto<string> result = ResultDto<string>.FailureResult("Failed to assign role");
            return BadRequest(result);
        }
        return Ok(ResultDto<string>.SuccessResult("Role assigned successfully"));
    }
}
