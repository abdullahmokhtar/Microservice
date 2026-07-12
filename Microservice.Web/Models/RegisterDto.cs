namespace Microservice.Web.Models;

public record RegisterDto(
    string Email,
    string Name,
    string Password,
    string PhoneNumber,
    string? Role);
