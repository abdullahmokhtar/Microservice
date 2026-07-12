namespace Microservices.service.AuthAPI.Models.Dto
{
    public record RegisterDto(
        string Email,
        string Name,
        string Password,
        string PhoneNumber,
        string? Role);
}
