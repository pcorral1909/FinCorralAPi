namespace FinCorralApi.Application.Common.Features;

public interface IFeatureFlagService
{
    bool IsEnabled(string feature);
}