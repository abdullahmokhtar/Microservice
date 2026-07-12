using Microservices.service.AuthAPI.Models.Dto;

namespace Microservices.service.AuthAPI.Services.Iservices;

public interface IAuthService
{
    public Task<string> RegisterAsync(RegisterDto registerDto);
    public Task<LoginResponseDto> LoginAsync(LoginDto loginDto);
    public Task<bool> AssignRoleAsync(string email, string roleName);
}
