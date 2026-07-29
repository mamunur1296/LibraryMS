using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using LibraryMS.Application.Behaviours;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LibraryMS.Application.Tests;

public class PipelineBehaviorsTests
{
    // Test command for ValidationBehavior
    public record TestCommand(string Name) : IRequest<string>;

    [Fact]
    public async Task ValidationBehavior_WithValidRequest_ShouldCallNext()
    {
        // Arrange
        var request = new TestCommand("Valid Name");
        var mockValidator = new Mock<IValidator<TestCommand>>();
        mockValidator.Setup(v => v.Validate(It.IsAny<ValidationContext<TestCommand>>()))
            .Returns(new ValidationResult()); // No errors

        var validators = new List<IValidator<TestCommand>> { mockValidator.Object };
        var loggerMock = new Mock<ILogger<ValidationBehavior<TestCommand, string>>>();
        var behavior = new ValidationBehavior<TestCommand, string>(validators, loggerMock.Object);

        var nextCalled = false;
        RequestHandlerDelegate<string> next = (ct) =>
        {
            nextCalled = true;
            return Task.FromResult("Success");
        };

        // Act
        var result = await behavior.Handle(request, next, CancellationToken.None);

        // Assert
        result.Should().Be("Success");
        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task ValidationBehavior_WithInvalidRequest_ShouldThrowValidationException()
    {
        // Arrange
        var request = new TestCommand("");
        var mockValidator = new Mock<IValidator<TestCommand>>();
        var failures = new List<ValidationFailure> { new("Name", "Name is required") };
        mockValidator.Setup(v => v.Validate(It.IsAny<ValidationContext<TestCommand>>()))
            .Returns(new ValidationResult(failures));

        var validators = new List<IValidator<TestCommand>> { mockValidator.Object };
        var loggerMock = new Mock<ILogger<ValidationBehavior<TestCommand, string>>>();
        var behavior = new ValidationBehavior<TestCommand, string>(validators, loggerMock.Object);

        RequestHandlerDelegate<string> next = (ct) => Task.FromResult("Success");

        // Act
        var act = () => behavior.Handle(request, next, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Domain.Shared.Exceptions.ValidationException>();
    }

    // Test command and behavior for RetryBehavior
    public record RetryableTestCommand(string Data) : IRequest<string>, IRetryableRequest;

    [Fact]
    public async Task RetryBehavior_WhenTransientExceptionOccurs_ShouldRetryAndEventuallySucceed()
    {
        // Arrange
        var request = new RetryableTestCommand("Test");
        var loggerMock = new Mock<ILogger<RetryBehavior<RetryableTestCommand, string>>>();
        var behavior = new RetryBehavior<RetryableTestCommand, string>(loggerMock.Object);

        var calls = 0;
        RequestHandlerDelegate<string> next = (ct) =>
        {
            calls++;
            if (calls < 2)
            {
                throw new Exception("Transient database failure");
            }
            return Task.FromResult("Success");
        };

        // Act
        var result = await behavior.Handle(request, next, CancellationToken.None);

        // Assert
        result.Should().Be("Success");
        calls.Should().Be(2); // Retried once and then succeeded
    }
}
