namespace LibraryMS.Domain.Common;

// Aggregate root — the entry point for a consistency boundary.
// Holds and dispatches domain events.
public abstract class AggregateRoot<TId> : Entity<TId>
{
    private readonly List<IDomainEvent> _domainEvents = [];

    // Read-only view of uncommitted domain events.
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected AggregateRoot() { }

    protected AggregateRoot(TId id) : base(id) { }

    // Registers a domain event to be dispatched after the transaction commits.
    protected void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    // Clears all domain events after they have been dispatched.
    public void ClearDomainEvents() => _domainEvents.Clear();
}
