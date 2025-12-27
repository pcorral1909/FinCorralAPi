using Microsoft.Extensions.Configuration;
using FinCorralApi.Application.Common.Features;

namespace FinCorralApi.Infrastructure.Features;

public sealed class ConfigurationFeatureFlagService
    : IFeatureFlagService
{
    private readonly IConfiguration _configuration;

    public ConfigurationFeatureFlagService(
        IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public bool IsEnabled(string feature)
    {
        return _configuration.GetValue<bool>(
            $"Features:{feature}");
    }
}

