using Microservice.Web.Models;

namespace Microservice.Web.Services.IServices;

public interface IAuthService
{
    public Task<ResultDto<LoginResponseDto>> LoginAsync(LoginDto loginDto);
    public Task<ResultDto<RegisterDto>> RegisterAsync(RegisterDto registerDto);
    public Task<ResultDto<RegisterDto>> AssignRoleAsync(RegisterDto registerDto);
}
