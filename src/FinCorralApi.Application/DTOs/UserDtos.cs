using System;

namespace FinCorralApi.Application.DTOs
{
    public enum NotificationChannel
    {
        Sms,
        WhatsApp,
        Email
    }

    public sealed class ValidateAltaDto
    {
        public string NumeroAlta { get; init; } = string.Empty;
    }

    public sealed class ValidateContactDto
    {
        public string Phone { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
    }

    public sealed class SendOtpDto
    {
        public string Destination { get; init; } = string.Empty;
        public NotificationChannel Channel { get; init; }
    }

    public sealed class VerifyOtpDto
    {
        public string Destination { get; init; } = string.Empty;
        public NotificationChannel Channel { get; init; }
        public string Otp { get; init; } = string.Empty;
    }

    public sealed class SetPasswordDto
    {
        public Guid UserId { get; init; }
        public string Password { get; init; } = string.Empty;
    }

    public sealed class CompleteRegistrationDto
    {
        public Guid UserId { get; init; }
        public string FullName { get; init; } = string.Empty;
        public string Curp { get; init; } = string.Empty;
        public DateTime BirthDate { get; init; }
    }

    public sealed class RecoverPasswordDto
    {
        public string Destination { get; init; } = string.Empty;
        public NotificationChannel Channel { get; init; }
    }
}