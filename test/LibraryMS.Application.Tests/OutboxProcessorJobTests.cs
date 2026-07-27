using FluentAssertions;
using LibraryMS.Domain.Common;
using LibraryMS.EntityFrameworkCore;
using LibraryMS.EntityFrameworkCore.Interceptors;
using LibraryMS.EntityFrameworkCore.Outbox;
using LibraryMS.Infrastructure.Jobs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LibraryMS.Application.Tests;

public class OutboxProcessorJobTests
{
    // Dummy domain event for outbox serialization/deserialization test
    public record SampleDomainEvent(Guid Id) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    private static LibraryDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var auditableInterceptor = new AuditableEntityInterceptor();
        var outboxInterceptor = new DomainEventToOutboxInterceptor();

        return new LibraryDbContext(options, auditableInterceptor, outboxInterceptor);
    }

    [Fact]
    public async Task ProcessAsync_WithPendingEligibleMessages_ShouldPublishAndMarkProcessed()
    {
        // Arrange
        using var dbContext = CreateDbContext();
        var sampleEvent = new SampleDomainEvent(Guid.NewGuid());
        var jsonContent = JsonSerializer.Serialize(sampleEvent);
        var typeName = typeof(SampleDomainEvent).FullName ?? typeof(SampleDomainEvent).Name;

        var outboxMessage = OutboxMessage.Create(
            typeName,
            jsonContent);

        dbContext.OutboxMessages.Add(outboxMessage);
        await dbContext.SaveChangesAsync();

        var publisherMock = new Mock<IPublisher>();
        var loggerMock = new Mock<ILogger<OutboxProcessorJob>>();
        var job = new OutboxProcessorJob(dbContext, publisherMock.Object, loggerMock.Object);

        // Act
        await job.ProcessAsync(CancellationToken.None);

        // Assert
        publisherMock.Verify(p => p.Publish(It.IsAny<IDomainEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        
        // Reload from database to assert state changes
        var updatedMessage = await dbContext.OutboxMessages.FindAsync(outboxMessage.Id);
        updatedMessage.Should().NotBeNull();
        updatedMessage!.ProcessedOn.Should().NotBeNull();
        updatedMessage.Error.Should().BeNull();
        updatedMessage.IsEligibleForProcessing.Should().BeFalse();
    }

    [Fact]
    public async Task ProcessAsync_WhenPublisherFails_ShouldIncrementRetryCountAndRecordError()
    {
        // Arrange
        using var dbContext = CreateDbContext();
        var sampleEvent = new SampleDomainEvent(Guid.NewGuid());
        var jsonContent = JsonSerializer.Serialize(sampleEvent);
        var typeName = typeof(SampleDomainEvent).FullName ?? typeof(SampleDomainEvent).Name;

        var outboxMessage = OutboxMessage.Create(
            typeName,
            jsonContent);

        dbContext.OutboxMessages.Add(outboxMessage);
        await dbContext.SaveChangesAsync();

        var publisherMock = new Mock<IPublisher>();
        publisherMock.Setup(p => p.Publish(It.IsAny<IDomainEvent>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("MediatR handler failed"));

        var loggerMock = new Mock<ILogger<OutboxProcessorJob>>();
        var job = new OutboxProcessorJob(dbContext, publisherMock.Object, loggerMock.Object);

        // Act
        await job.ProcessAsync(CancellationToken.None);

        // Assert
        var updatedMessage = await dbContext.OutboxMessages.FindAsync(outboxMessage.Id);
        updatedMessage.Should().NotBeNull();
        updatedMessage!.ProcessedOn.Should().BeNull();
        updatedMessage.RetryCount.Should().Be(1);
        updatedMessage.Error.Should().Contain("MediatR handler failed");
        updatedMessage.IsEligibleForProcessing.Should().BeTrue(); // Can still retry
    }

    [Fact]
    public async Task ProcessAsync_WhenMessageReachesMaxRetries_ShouldBecomeDeadLetter()
    {
        // Arrange
        using var dbContext = CreateDbContext();
        var sampleEvent = new SampleDomainEvent(Guid.NewGuid());
        var jsonContent = JsonSerializer.Serialize(sampleEvent);
        var typeName = typeof(SampleDomainEvent).FullName ?? typeof(SampleDomainEvent).Name;

        var outboxMessage = OutboxMessage.Create(
            typeName,
            jsonContent);

        // Simulate 2 failed attempts (since MaxRetries is 3)
        for (int i = 0; i < 2; i++)
        {
            outboxMessage.RecordFailure("Attempt failed");
        }

        dbContext.OutboxMessages.Add(outboxMessage);
        await dbContext.SaveChangesAsync();

        var publisherMock = new Mock<IPublisher>();
        publisherMock.Setup(p => p.Publish(It.IsAny<IDomainEvent>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("MediatR handler failed"));

        var loggerMock = new Mock<ILogger<OutboxProcessorJob>>();
        var job = new OutboxProcessorJob(dbContext, publisherMock.Object, loggerMock.Object);

        // Act
        await job.ProcessAsync(CancellationToken.None);

        // Assert
        var updatedMessage = await dbContext.OutboxMessages.FindAsync(outboxMessage.Id);
        updatedMessage.Should().NotBeNull();
        updatedMessage!.RetryCount.Should().Be(3);
        updatedMessage.IsDeadLetter.Should().BeTrue();
        updatedMessage.IsEligibleForProcessing.Should().BeFalse(); // Stopped retrying
    }
}
