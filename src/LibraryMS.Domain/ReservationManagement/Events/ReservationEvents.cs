using LibraryMS.Domain.Common;

namespace LibraryMS.Domain.ReservationManagement.Events;

public sealed record ReservationCreatedEvent(
    Guid ReservationId, Guid MemberId, Guid BookId, int QueuePosition) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public sealed record ReservationAvailableEvent(
    Guid ReservationId, Guid MemberId, Guid BookId, DateTime ExpiresAt) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public sealed record ReservationCancelledEvent(
    Guid ReservationId, Guid MemberId, Guid BookId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public sealed record ReservationExpiredEvent(
    Guid ReservationId, Guid MemberId, Guid BookId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
