namespace FinCorralApi.Application.DTOs;

public record LoginDto(string Email, string Password);
public record LoginResponseDto(string Token, string RefreshToken, DateTime Expires);
public record RefreshTokenDto(string RefreshToken);