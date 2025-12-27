using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using FinCorralApi.Application.Interfaces;
using FinCorralApi.Application.DTOs;
using FinCorralApi.Shared;

namespace FinCorralApi.Infrastructure.Services
{
    public sealed class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(ILogger<NotificationService> logger)
        {
            _logger = logger;
        }

        public Task<Result> SendOtpAsync(string destination, NotificationChannel channel, string otp, CancellationToken cancellationToken)
        {
            // Stub: replace with provider SDK (Twilio, Vonage, WhatsApp Business API, SendGrid)
            _logger.LogInformation("Enviando OTP por {Channel} a {Destination}. OTP (no almacenar en texto claro): {Otp}", channel, destination, otp);
            return Task.FromResult(Result.Ok());
        }

        public Task<Result> SendMessageAsync(string destination, NotificationChannel channel, string subject, string body, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Enviando mensaje por {Channel} a {Destination}: {Subject}", channel, destination, subject);
            return Task.FromResult(Result.Ok());
        }
    }
}