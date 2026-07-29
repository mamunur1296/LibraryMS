using LibraryMS.Domain.Common;
using System;

namespace LibraryMS.Domain.BorrowManagement.Events;

public sealed record BookBorrowedEvent(
    Guid BorrowId, Guid MemberId, Guid CopyId, Guid BookId, DateTime DueDate) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
