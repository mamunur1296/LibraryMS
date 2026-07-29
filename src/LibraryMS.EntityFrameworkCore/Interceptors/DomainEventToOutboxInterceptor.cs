using LibraryMS.Domain.Common;
using LibraryMS.EntityFrameworkCore.Outbox;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.Json;

namespace LibraryMS.EntityFrameworkCore.Interceptors;

// Interceptor that implements the Transactional Outbox Pattern.
// Instead of publishing domain events immediately (which breaks atomicity),
// it serializes them into the OutboxMessages table within the same DB transaction.
// A separate Hangfire background job (OutboxProcessorJob) polls and dispatches them.
public sealed class DomainEventToOutboxInterceptor : SaveChangesInterceptor
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false
    };

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var dbContext = eventData.Context;
        if (dbContext is null)
            return base.SavingChangesAsync(eventData, result, cancellationToken);

        // Gather all domain events from all aggregates being tracked
        var entitiesWithEvents = dbContext.ChangeTracker.Entries()
            .Select(e => e.Entity)
            .OfType<AggregateRoot<Guid>>()
            .Where(e => e.DomainEvents.Any())
            .ToList();

        var outboxMessages = entitiesWithEvents
            .SelectMany(e => e.DomainEvents)
            .Select(domainEvent => OutboxMessage.Create(
                type: domainEvent.GetType().FullName ?? domainEvent.GetType().Name,
                content: JsonSerializer.Serialize(domainEvent, domainEvent.GetType(), JsonOptions)))
            .ToList();

        // Clear events before save so they are not processed again
        entitiesWithEvents.ForEach(e => e.ClearDomainEvents());

        // Add to OutboxMessages table — persisted in the SAME transaction as business data
        if (outboxMessages.Count > 0)
        {
            dbContext.Set<OutboxMessage>().AddRange(outboxMessages);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
