using LibraryMS.Domain.Common;
using LibraryMS.EntityFrameworkCore;
using LibraryMS.EntityFrameworkCore.Outbox;
using LibraryMS.Infrastructure.Jobs.Handlers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace LibraryMS.Infrastructure.Jobs;

// Hangfire recurring job that implements the Outbox Processor.
public sealed class OutboxProcessorJob
{
    private readonly LibraryDbContext _dbContext;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxProcessorJob> _logger;
    private readonly IOutboxMessageHandler _messageHandlerChain;

    public OutboxProcessorJob(
        LibraryDbContext dbContext,
        IServiceProvider serviceProvider,
        ILogger<OutboxProcessorJob> logger)
    {
        _dbContext = dbContext;
        _serviceProvider = serviceProvider;
        _logger = logger;

        // Build the chain
        var domainHandler = new DomainEventOutboxMessageHandler(
            serviceProvider.GetRequiredService<IPublisher>(), 
            serviceProvider.GetRequiredService<ILogger<DomainEventOutboxMessageHandler>>());
            
        var emailHandler = new EmailOutboxMessageHandler(
            serviceProvider.GetRequiredService<ILogger<EmailOutboxMessageHandler>>());

        domainHandler.SetNext(emailHandler);
        
        _messageHandlerChain = domainHandler;
    }

    // Entry point called by Hangfire every 30 seconds.
    // Fetches all eligible (unprocessed, non-dead) outbox messages and processes them.
    public async Task ProcessAsync(CancellationToken cancellationToken = default)
    {
        var messages = await _dbContext.OutboxMessages
            .Where(m => m.ProcessedOn == null && m.RetryCount < m.MaxRetries)
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

            var handled = await _messageHandlerChain.HandleAsync(message, domainEvent, cancellationToken);
            
            if (handled)
            {
                message.MarkAsProcessed();
                _logger.LogInformation("OutboxProcessor: Processed message {Id} ({Type}) with Category: {Category}.", message.Id, message.Type, message.Category ?? "None");
            }
            else
            {
                _logger.LogWarning("OutboxProcessor: Message {Id} was not handled by any handler in the chain.", message.Id);
                message.RecordFailure("No handler found for message.");
            }

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
