namespace LibraryMS.Domain.Common;

/// <summary>
/// Aggregate root — the entry point for a consistency boundary.
/// Holds and dispatches domain events.
/// </summary>
/// <typeparam name="TId">Type of the aggregate's unique identifier.</typeparam>
public abstract class AggregateRoot<TId> : Entity<TId>
{
    private readonly List<IDomainEvent> _domainEvents = [];

    /// <summary>Read-only view of uncommitted domain events.</summary>
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected AggregateRoot() { }

    protected AggregateRoot(TId id) : base(id) { }

    /// <summary>Registers a domain event to be dispatched after the transaction commits.</summary>
    protected void AddDomainEvent(IDomainEvent domainEvent)
        => _domainEvents.Add(domainEvent);

    /// <summary>Clears all domain events after they have been dispatched.</summary>
    public void ClearDomainEvents()
        => _domainEvents.Clear();
}
