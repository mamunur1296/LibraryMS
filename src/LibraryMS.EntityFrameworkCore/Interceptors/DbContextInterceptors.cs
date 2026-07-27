using LibraryMS.Domain.Shared.Interfaces;
using LibraryMS.Domain.Common;
using LibraryMS.EntityFrameworkCore.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.Json;

namespace LibraryMS.EntityFrameworkCore.Interceptors;

public sealed class AuditableEntityInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void UpdateEntities(DbContext? context)
    {
        if (context is null) return;

        var entries = context.ChangeTracker.Entries<IAuditableEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.LastModifiedAt = DateTime.UtcNow;
            }
        }

        var softDeleteEntries = context.ChangeTracker.Entries<ISoftDelete>();
        foreach (var entry in softDeleteEntries)
        {
            if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
                entry.Entity.DeletedAt = DateTime.UtcNow;
            }
        }
    }
}

/// <summary>
/// Interceptor that implements the Transactional Outbox Pattern.
/// Instead of publishing domain events immediately (which breaks atomicity),
/// it serializes them into the OutboxMessages table within the same DB transaction.
/// A separate Hangfire background job (OutboxProcessorJob) polls and dispatches them.
/// </summary>
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
