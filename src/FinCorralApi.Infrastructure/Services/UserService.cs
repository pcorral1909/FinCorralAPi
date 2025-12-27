using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using FinCorralApi.Application.Interfaces;
using FinCorralApi.Application.DTOs;
using FinCorralApi.Shared;

namespace FinCorralApi.Infrastructure.Services
{
    public sealed class UserService : IUserService
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<UserService> _logger;

        public UserService(INotificationService notificationService, ILogger<UserService> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        public Task<Result> ValidateNumeroAltaAsync(string numeroAlta, CancellationToken cancellationToken)
        {
            // Ejemplo básico: validar formato y existencia en tabla de altas
            if (string.IsNullOrWhiteSpace(numeroAlta))
            {
                return Task.FromResult(Result.Fail("InvalidInput", "El número de alta es obligatorio."));
            }

            // TODO: consultar base de datos (Azure SQL) para verificar número de alta
            var exists = true; // placeholder
            return Task.FromResult(exists ? Result.Ok() : Result.Fail("NotFound", "Número de alta no válido."));
        }

        public Task<Result> ValidateContactAsync(string phone, string email, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(phone) && string.IsNullOrWhiteSpace(email))
            {
                return Task.FromResult(Result.Fail("InvalidInput", "Teléfono o correo requerido."));
            }

            // TODO: validaciones adicionales (regex, duplicados en DB)
            return Task.FromResult(Result.Ok());
        }

        public async Task<Result> SendOtpAsync(string destination, NotificationChannel channel, CancellationToken cancellationToken)
        {
            // Generar OTP (6 dígitos) y almacenarlo hashed en DB con expiración
            var otp = new Random().Next(100000, 999999).ToString();
            _logger.LogDebug("OTP generado para {Destination}", destination);

            // TODO: almacenar OTP (hash) en base de datos con expiración

            var notifyResult = await _notificationService.SendOtpAsync(destination, channel, otp, cancellationToken).ConfigureAwait(false);
            if (!notifyResult.IsSuccess) return notifyResult;

            return Result.Ok();
        }

        public Task<Result> VerifyOtpAsync(string destination, NotificationChannel channel, string otp, CancellationToken cancellationToken)
        {
            // TODO: comprobar OTP comparando hash en DB y expiración
            var matches = true; // placeholder
            return Task.FromResult(matches ? Result.Ok() : Result.Fail("InvalidOtp", "OTP inválido o expirado."));
        }

        public Task<Result<Guid>> CreateUserAccountAsync(string phone, string email, CancellationToken cancellationToken)
        {
            // TODO: insertar usuario provisional en DB y devolver userId
            var userId = Guid.NewGuid();
            _logger.LogInformation("Usuario provisional creado {UserId} para {Phone} / {Email}", userId, phone, email);
            return Task.FromResult(Result<Guid>.Ok(userId));
        }

        public Task<Result> SetPasswordAsync(Guid userId, string password, CancellationToken cancellationToken)
        {
            if (userId == Guid.Empty) return Task.FromResult(Result.Fail("InvalidInput", "UserId inválido."));
            if (string.IsNullOrWhiteSpace(password)) return Task.FromResult(Result.Fail("InvalidInput", "Password requerido."));

            // TODO: hashear password (bcrypt/Argon2) y guardar en DB
            _logger.LogInformation("Contraseña establecida para {UserId}", userId);
            return Task.FromResult(Result.Ok());
        }

        public Task<Result> CompleteRegistrationAsync(CompleteRegistrationDto dto, CancellationToken cancellationToken)
        {
            if (dto.UserId == Guid.Empty) return Task.FromResult(Result.Fail("InvalidInput", "UserId inválido."));
            // TODO: validar CURP, fecha de nacimiento y actualizar entidad Usuario en DB
            _logger.LogInformation("Registro completo para {UserId}", dto.UserId);
            return Task.FromResult(Result.Ok());
        }

        public async Task<Result> RecoverPasswordAsync(string destination, NotificationChannel channel, CancellationToken cancellationToken)
        {
            // Generar OTP y enviar para recuperación
            var otpResult = await SendOtpAsync(destination, channel, cancellationToken).ConfigureAwait(false);
            return otpResult;
        }
    }
}