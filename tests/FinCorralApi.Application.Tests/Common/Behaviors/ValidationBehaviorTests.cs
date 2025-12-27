using FluentAssertions;
using FluentValidation;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FinCorralApi.Application.Behaviors;
using FinCorralApi.Application.Command.CreateSample;
using FinCorralApi.Shared.Results;
using Xunit;

namespace FinCorralApi.Application.Tests.Behaviors;

public class ValidationBehaviorTests
{
    private sealed class DummyValidator
        : AbstractValidator<CreateSampleCommand>
    {
        public DummyValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }

    [Fact]
    public async Task Valid_Request_Should_Invoke_Next()
    {
        var validators = new List<IValidator<CreateSampleCommand>>
        {
            new DummyValidator()
        };

        var behavior = new ValidationBehavior<CreateSampleCommand, Result<Guid>>(validators);

        var request = new CreateSampleCommand("Valid");

        var called = false;

        RequestHandlerDelegate<Result<Guid>> next = () =>
        {
            called = true;
            return Task.FromResult(Result<Guid>.Success(Guid.NewGuid()));
        };

        var result = await behavior.Handle(request, next, CancellationToken.None);

        called.Should().BeTrue();
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Invalid_Request_Should_Throw_ValidationException_And_Not_Invoke_Next()
    {
        var validators = new List<IValidator<CreateSampleCommand>>
        {
            new DummyValidator()
        };

        var behavior = new ValidationBehavior<CreateSampleCommand, Result<Guid>>(validators);

        var request = new CreateSampleCommand(string.Empty);

        var called = false;

        RequestHandlerDelegate<Result<Guid>> next = () =>
        {
            called = true;
            return Task.FromResult(Result<Guid>.Success(Guid.NewGuid()));
        };

        // Act
        Func<Task> act = async () =>
            await behavior.Handle(request, next, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
        called.Should().BeFalse();
    }
}