using LibraryMS.Domain.Common;

namespace LibraryMS.Domain.BookManagement.Events;

public sealed record BookCreatedEvent(Guid BookId, string Title, string ISBN) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public sealed record BookCopyAddedEvent(Guid BookId, Guid CopyId, Guid BranchId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
