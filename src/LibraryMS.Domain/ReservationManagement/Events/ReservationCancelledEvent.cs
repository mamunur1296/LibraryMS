using LibraryMS.Domain.Common;
using System;

namespace LibraryMS.Domain.ReservationManagement.Events;

public sealed record ReservationCancelledEvent(
    Guid ReservationId, Guid MemberId, Guid BookId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
