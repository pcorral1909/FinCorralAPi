using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using FinCorralApi.Application.Interfaces;
using FinCorralApi.Shared.Results;
using FinCorralApi.Domain.Enums;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace FinCorralApi.Infrastructure.Services;

public sealed class TwilioNotificationService : INotificationService
{
    private readonly TwilioOptions _options;
    private readonly ILogger<TwilioNotificationService> _logger;

    public TwilioNotificationService(IOptions<TwilioOptions> options, ILogger<TwilioNotificationService> logger)
    {
        _options = options.Value;
        _logger = logger;
        // Inicializar Twilio client con credenciales desde configuraci�n (recomendado: KeyVault)
        TwilioClient.Init(_options.AccountSid, _options.AuthToken);
    }

    public Task<Result> SendOtpAsync(string destination, NotificationChannel channel, string otp, CancellationToken cancellationToken)
    {
        // Para WhatsApp: use "whatsapp:+123..." como To
        var body = $"Tu c�digo OTP es: {otp}";
        return SendMessageAsync(destination, channel, "OTP", body, cancellationToken);
    }

    public Task<Result> SendMessageAsync(string destination, NotificationChannel channel, string subject, string body, CancellationToken cancellationToken)
    {
        try
        {
            var from = new PhoneNumber(_options.From);

            var message = MessageResource.Create(
                body: body,
                from: from,
                to: new PhoneNumber(destination)
            );

            _logger.LogInformation("Twilio message sid {Sid} to {Dest}", message.Sid, destination);
            return Task.FromResult(Result.Success());
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Error enviando mensaje Twilio a {Dest}", destination);
            return Task.FromResult(Result.Failure(new Error("SmsError", "Error enviando SMS/WhatsApp")));
        }
    }
}

public sealed class TwilioOptions
{
    public string AccountSid { get; set; } = string.Empty;
    public string AuthToken { get; set; } = string.Empty;
    public string From { get; set; } = string.Empty;
}