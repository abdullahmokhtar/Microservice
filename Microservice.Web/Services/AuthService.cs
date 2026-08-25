using Microservice.Web.Models;
using Microservice.Web.Services.IServices;
using Microservice.Web.Utility;

namespace Microservice.Web.Services;

public class AuthService(IBaseService baseService, ServicesBaseURI servicesBaseURI) : IAuthService
{
    public async Task<ResultDto<RegisterDto>> AssignRoleAsync(RegisterDto registerDto)
    {
        return await baseService.SendAsync<RegisterDto>(new RequestDto
        {
            APIType = ApiType.POST,
            Data = registerDto,
            URL = servicesBaseURI.AuthAPI + "/api/auth/assign-role"
        });
    }

    public async Task<ResultDto<LoginResponseDto>> LoginAsync(LoginDto loginDto)
    {
        return await baseService.SendAsync<LoginResponseDto>(new RequestDto
        {
            APIType = ApiType.POST,
            Data = loginDto,
            URL = servicesBaseURI.AuthAPI + "/api/auth/login"
        });
    }

    public async Task<ResultDto<RegisterDto>> RegisterAsync(RegisterDto registerDto)
    {
        return await baseService.SendAsync<RegisterDto>(new RequestDto
        {
            APIType = ApiType.POST,
            Data = registerDto,
            URL = servicesBaseURI.AuthAPI + "/api/auth/register"
        });
    }
}
