using System;
using System.Threading;
using System.Threading.Tasks;
using FinCorralApi.Shared;
using FinCorralApi.Shared.Results;

namespace FinCorralApi.Application.Interfaces;

public interface IOtpService
{
    Task<Result> GenerateAndSendOtpAsync(string destination, string purpose, CancellationToken cancellationToken);
    Task<Result> VerifyOtpAsync(string destination, string purpose, string otp, CancellationToken cancellationToken);
}