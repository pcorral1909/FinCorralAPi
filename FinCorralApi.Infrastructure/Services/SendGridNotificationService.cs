using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using FinCorralApi.Application.Interfaces;
using FinCorralApi.Shared.Results;
using FinCorralApi.Domain.Enums;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace FinCorralApi.Infrastructure.Services;

public sealed class SendGridNotificationService : INotificationService
{
    private readonly SendGridOptions _options;
    private readonly ILogger<SendGridNotificationService> _logger;
    private readonly SendGridClient _client;

    public SendGridNotificationService(IOptions<SendGridOptions> options, ILogger<SendGridNotificationService> logger)
    {
        _options = options.Value;
        _logger = logger;
        _client = new SendGridClient(_options.ApiKey);
    }

    public async Task<Result> SendOtpAsync(string destination, NotificationChannel channel, string otp, CancellationToken cancellationToken)
    {
        var subject = "C�digo OTP";
        var body = $"Tu c�digo OTP es: {otp}";
        return await SendMessageAsync(destination, channel, subject, body, cancellationToken).ConfigureAwait(false);
    }

    public async Task<Result> SendMessageAsync(string destination, NotificationChannel channel, string subject, string body, CancellationToken cancellationToken)
    {
        try
        {
            var from = new EmailAddress(_options.FromEmail, _options.FromName);
            var to = new EmailAddress(destination);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, body, body);
            var response = await _client.SendEmailAsync(msg, cancellationToken).ConfigureAwait(false);
            if (response.IsSuccessStatusCode) return Result.Success();

            _logger.LogWarning("SendGrid returned {StatusCode} when sending to {Dest}", response.StatusCode, destination);
            return Result.Failure(new Error("EmailError", "No se pudo enviar el correo."));
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Error enviando email a {Dest}", destination);
            return Result.Failure(new Error("EmailError", "Error al enviar correo."));
        }
    }
}

public sealed class SendGridOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
}