using LibraryMS.Domain.Common;

namespace LibraryMS.Domain.BorrowManagement.Events;

public sealed record BookBorrowedEvent(
    Guid BorrowId, Guid MemberId, Guid CopyId, Guid BookId, DateTime DueDate) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public sealed record BookReturnedEvent(
    Guid BorrowId, Guid MemberId, Guid CopyId, Guid BookId, decimal LateFine) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public sealed record BorrowOverdueEvent(
    Guid BorrowId, Guid MemberId, Guid BookId, DateTime DueDate) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
