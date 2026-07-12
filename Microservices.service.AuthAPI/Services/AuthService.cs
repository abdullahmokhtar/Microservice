using Microservices.service.AuthAPI.Models;
using Microservices.service.AuthAPI.Models.Dto;
using Microservices.service.AuthAPI.Services.Iservices;
using Microsoft.AspNetCore.Identity;

namespace Microservices.service.AuthAPI.Services;

public class AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IJWTTokenGenerator jWTTokenGenerator) : IAuthService
{
    public async Task<bool> AssignRoleAsync(string email, string roleName)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user is null)
            return false;

        var role = await roleManager.FindByNameAsync(roleName);
        if (role is null)
        {
            var result = await roleManager.CreateAsync(new IdentityRole(roleName));
            if (!result.Succeeded)
                return false;
        }
        var resultAssignRole = await userManager.AddToRoleAsync(user, roleName);
        return resultAssignRole.Succeeded;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user = await userManager.FindByNameAsync(loginDto.Username);

        var isPasswordValid = await userManager.CheckPasswordAsync(user, loginDto.Password);

        if (!isPasswordValid)
        {
            return new LoginResponseDto(string.Empty, null);
        }

        var userDto = new UserDto(user.Id, user.Email, user.Name, user.PhoneNumber);
        var token = jWTTokenGenerator.GenerateToken(user);
        var response = new LoginResponseDto(token, userDto);

        return response;
    }

    public async Task<string> RegisterAsync(RegisterDto registerDto)
    {
        var user = new ApplicationUser
        {
            UserName = registerDto.Email,
            Email = registerDto.Email,
            Name = registerDto.Name,
            PhoneNumber = registerDto.PhoneNumber
        };

        var result = await userManager.CreateAsync(user, registerDto.Password);

        if (result.Succeeded)
        {
            return string.Empty;
        }
        return string.Join(',', result.Errors.Select(e => e.Description));
    }
}
