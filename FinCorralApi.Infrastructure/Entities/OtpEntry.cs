using System;

namespace FinCorralApi.Infrastructure.Entities;

public sealed class OtpEntry
{
    public Guid Id { get; set; }
    public string Destination { get; set; } = string.Empty; // phone or email
    public string Purpose { get; set; } = string.Empty; // e.g. "onboarding", "recover"
    public string OtpHash { get; set; } = string.Empty; // store hash of otp
    public DateTime ExpiresAt { get; set; }
    public bool Consumed { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}