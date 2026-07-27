using MediatR;

namespace LibraryMS.Domain.Common;

/// <summary>
/// Marker interface for all domain events.
/// Implements MediatR INotification for in-process dispatching.
/// </summary>
public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
}
