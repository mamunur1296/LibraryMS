using LibraryMS.EntityFrameworkCore;
using LibraryMS.EntityFrameworkCore.Outbox;
using LibraryMS.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace LibraryMS.Infrastructure.Jobs;

/// <summary>
/// Hangfire recurring job that implements the Outbox Processor.
/// Polls OutboxMessages table, deserializes domain events, and publishes
/// them via MediatR. Implements the Retry Mechanism:
///   - On success: marks message as ProcessedOn = UtcNow
///   - On failure: increments RetryCount, saves error
///   - Dead letters (RetryCount >= MaxRetries) are skipped permanently
/// </summary>
public sealed class OutboxProcessorJob
{
    private readonly LibraryDbContext _dbContext;
    private readonly IPublisher _publisher;
    private readonly ILogger<OutboxProcessorJob> _logger;

    public OutboxProcessorJob(
        LibraryDbContext dbContext,
        IPublisher publisher,
        ILogger<OutboxProcessorJob> logger)
    {
        _dbContext = dbContext;
        _publisher = publisher;
        _logger = logger;
    }

    /// <summary>
    /// Entry point called by Hangfire every 30 seconds.
    /// Fetches all eligible (unprocessed, non-dead) outbox messages and processes them.
    /// </summary>
    public async Task ProcessAsync(CancellationToken cancellationToken = default)
    {
        var messages = await _dbContext.OutboxMessages
            .Where(m => m.IsEligibleForProcessing)
            .OrderBy(m => m.OccurredOn)
            .Take(50)
            .ToListAsync(cancellationToken);

        if (messages.Count == 0)
        {
            _logger.LogDebug("OutboxProcessor: No pending messages found.");
            return;
        }

        _logger.LogInformation("OutboxProcessor: Processing {Count} outbox messages.", messages.Count);

        foreach (var message in messages)
        {
            await ProcessMessageAsync(message, cancellationToken);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task ProcessMessageAsync(OutboxMessage message, CancellationToken cancellationToken)
    {
        try
        {
            var eventType = ResolveEventType(message.Type);
            if (eventType is null)
            {
                _logger.LogError("OutboxProcessor: Could not resolve type '{Type}'.", message.Type);
                message.RecordFailure($"Cannot resolve type: {message.Type}");
                return;
            }

            var domainEvent = JsonSerializer.Deserialize(message.Content, eventType) as IDomainEvent;
            if (domainEvent is null)
            {
                message.RecordFailure("Deserialization returned null.");
                return;
            }

            await _publisher.Publish(domainEvent, cancellationToken);
            message.MarkAsProcessed();

            _logger.LogInformation("OutboxProcessor: Processed message {Id} ({Type}).", message.Id, message.Type);
        }
        catch (Exception ex)
        {
            message.RecordFailure(ex.Message);

            if (message.IsDeadLetter)
                _logger.LogError(ex, "OutboxProcessor: Message {Id} is now a dead letter after {Retries} retries.", message.Id, message.RetryCount);
            else
                _logger.LogWarning(ex, "OutboxProcessor: Message {Id} failed (attempt {Retries}). Will retry.", message.Id, message.RetryCount);
        }
    }

    private static Type? ResolveEventType(string typeName)
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            var type = assembly.GetType(typeName);
            if (type is not null) return type;
        }
        return null;
    }
}
