using FluentAssertions;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using FinCorralApi.Application.Command.CreateSample;

using FinCorralApi.Application.Common.Features;
using FinCorralApi.Shared.Features;
using FinCorralApi.Shared.Results;
using Xunit;

namespace FinCorralApi.Application.Tests.Commands.CreateSample;

public class CreateSampleHandlerTests
{
    private readonly Mock<IFeatureFlagService> _featureFlags;

    public CreateSampleHandlerTests()
    {
        _featureFlags = new Mock<IFeatureFlagService>();
    }

    [Fact]
    public async Task Handle_When_Feature_Disabled_Uses_V1()
    {
        // Arrange
        _featureFlags
            .Setup(x => x.IsEnabled(FeatureFlags.NewSampleFlow))
            .Returns(false);

        var handler = new CreateSampleHandler(_featureFlags.Object);
        var command = new CreateSampleCommand("Sample");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task Handle_When_Feature_Enabled_Uses_V2()
    {
        // Arrange
        _featureFlags
            .Setup(x => x.IsEnabled(FeatureFlags.NewSampleFlow))
            .Returns(true);

        var handler = new CreateSampleHandler(_featureFlags.Object);
        var command = new CreateSampleCommand("Sample");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBe(Guid.Empty);
    }
}
