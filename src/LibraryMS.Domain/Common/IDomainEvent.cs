using MediatR;

namespace LibraryMS.Domain.Common;

// Marker interface for all domain events.
// Implements MediatR INotification for in-process dispatching.
public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
}
