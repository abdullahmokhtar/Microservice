namespace Microservices.service.AuthAPI.Models.Dto;

public record LoginResponseDto(string Token, UserDto User);
