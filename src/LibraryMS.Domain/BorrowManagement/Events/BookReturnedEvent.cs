using LibraryMS.Domain.Common;
using System;

namespace LibraryMS.Domain.BorrowManagement.Events;

public sealed record BookReturnedEvent(
    Guid BorrowId, Guid MemberId, Guid CopyId, Guid BookId, decimal LateFine) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
