using System;
using System.Threading;
using System.Threading.Tasks;
using FinCorralApi.Application.DTOs;
using FinCorralApi.Shared.Results;
using FinCorralApi.Domain.Enums;

namespace FinCorralApi.Application.Interfaces
{
    public interface IUserService
    {
        Task<Result> ValidateNumeroAltaAsync(string numeroAlta, CancellationToken cancellationToken);
        Task<Result> ValidateContactAsync(string phone, string email, CancellationToken cancellationToken);
        Task<Result> SendOtpAsync(string destination, NotificationChannel channel, CancellationToken cancellationToken);
        Task<Result> VerifyOtpAsync(string destination, NotificationChannel channel, string otp, CancellationToken cancellationToken);
        Task<Result<Guid>> CreateUserAccountAsync(string phone, string email, CancellationToken cancellationToken);
        Task<Result> SetPasswordAsync(Guid userId, string password, CancellationToken cancellationToken);
        Task<Result> CompleteRegistrationAsync(CompleteRegistrationDto dto, CancellationToken cancellationToken);
        Task<Result> RecoverPasswordAsync(string destination, NotificationChannel channel, CancellationToken cancellationToken);
    }
}