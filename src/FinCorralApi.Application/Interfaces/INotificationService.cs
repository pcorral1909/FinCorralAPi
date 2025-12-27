using System.Threading;
using System.Threading.Tasks;
using FinCorralApi.Application.DTOs;
using FinCorralApi.Shared;

namespace FinCorralApi.Application.Interfaces
{
    public interface INotificationService
    {
        Task<Result> SendOtpAsync(string destination, NotificationChannel channel, string otp, CancellationToken cancellationToken);
        Task<Result> SendMessageAsync(string destination, NotificationChannel channel, string subject, string body, CancellationToken cancellationToken);
    }
}