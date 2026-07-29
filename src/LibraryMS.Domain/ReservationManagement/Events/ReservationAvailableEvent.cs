using LibraryMS.Domain.Common;
using System;

namespace LibraryMS.Domain.ReservationManagement.Events;

public sealed record ReservationAvailableEvent(
    Guid ReservationId, Guid MemberId, Guid BookId, DateTime ExpiresAt) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
