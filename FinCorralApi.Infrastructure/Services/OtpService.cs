using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using FinCorralApi.Application.Interfaces;
using FinCorralApi.Infrastructure.Data;
using FinCorralApi.Infrastructure.Entities;
using FinCorralApi.Shared.Results;
using FinCorralApi.Domain.Enums;

namespace FinCorralApi.Infrastructure.Services;

public sealed class OtpService : IOtpService
{
    private readonly AppDbContext _db;
    private readonly INotificationService _notification;
    private readonly IPasswordHasher _hasher;
    private readonly ILogger<OtpService> _logger;
    private readonly TimeSpan _otpTtl = TimeSpan.FromMinutes(5);

    public OtpService(AppDbContext db,
        INotificationService notification,
        IPasswordHasher hasher,
        ILogger<OtpService> logger)
    {
        _db = db;
        _notification = notification;
        _hasher = hasher;
        _logger = logger;
    }

    public async Task<Result> GenerateAndSendOtpAsync(string destination, string purpose, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(destination)) return Result.Failure(new Error("InvalidInput", "Destination requerido."));

        // Generar OTP numrico de 6 dgitos (seguro para la mayora de casos; para ms seguridad use RNGCrypto)
        var otp = RandomNumberGenerator.GetInt32(100000, 999999).ToString();

        // Hash OTP antes de persistir
        var otpHash = _hasher.Hash(otp);

        var entry = new OtpEntry
        {
            Destination = destination,
            Purpose = purpose,
            OtpHash = otpHash,
            ExpiresAt = DateTime.UtcNow.Add(_otpTtl),
            Consumed = false
        };

        await _db.OtpEntries.AddAsync(entry, cancellationToken).ConfigureAwait(false);
        await _db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        // Enviar notificacin (async)  decide canal en NotificationService segn formato de destination
        var sendResult = await _notification.SendOtpAsync(destination, DetermineChannel(destination), otp, cancellationToken).ConfigureAwait(false);
        if (!sendResult.IsSuccess)
        {
            _logger.LogWarning("Fallo envo OTP a {Destination}: {Error}", destination, sendResult.Error?.Message);
            return Result.Failure(new Error("NotificationFailed", "No se pudo enviar el OTP."));
        }

        return Result.Success();
    }

    public async Task<Result> VerifyOtpAsync(string destination, string purpose, string otp, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var entry = await _db.OtpEntries
            .FirstOrDefaultAsync(o => o.Destination == destination && o.Purpose == purpose && !o.Consumed && o.ExpiresAt >= now, cancellationToken)
            .ConfigureAwait(false);

        if (entry == null) return Result.Failure(new Error("NotFound", "OTP no encontrado o expirado."));

        var matches = _hasher.Verify(otp, entry.OtpHash);
        if (!matches) return Result.Failure(new Error("InvalidOtp", "OTP inválido."));

        entry.Consumed = true;
        _db.OtpEntries.Update(entry);
        await _db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Success();
    }

    private NotificationChannel DetermineChannel(string destination)
    {
        // Simple heuristic: if contains '@' => email, else SMS/WhatsApp default to Sms (NotificationService puede elegir WhatsApp si aplica)
        return destination.Contains("@") ? NotificationChannel.Email : NotificationChannel.Sms;
    }
}