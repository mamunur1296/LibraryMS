using LibraryMS.Domain.Common;
using System;

namespace LibraryMS.Domain.ReservationManagement.Events;

public sealed record ReservationCreatedEvent(
    Guid ReservationId, Guid MemberId, Guid BookId, int QueuePosition) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
