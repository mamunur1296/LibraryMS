using LibraryMS.Domain.Common;
using System;

namespace LibraryMS.Domain.BorrowManagement.Events;

public sealed record BorrowOverdueEvent(
    Guid BorrowId, Guid MemberId, Guid BookId, DateTime DueDate) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
