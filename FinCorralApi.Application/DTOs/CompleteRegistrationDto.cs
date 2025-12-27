namespace FinCorralApi.Application.DTOs;

public class CompleteRegistrationDto
{
    public Guid UserId { get; set; }
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}