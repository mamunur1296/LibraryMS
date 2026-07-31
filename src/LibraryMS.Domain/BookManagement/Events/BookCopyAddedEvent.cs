using LibraryMS.Domain.Common;

namespace LibraryMS.Domain.BookManagement.Events;

public sealed record BookCopyAddedEvent(Guid BookId, Guid CopyId, Guid BranchId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
