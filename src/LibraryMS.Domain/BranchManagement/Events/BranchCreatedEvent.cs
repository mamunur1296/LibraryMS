using LibraryMS.Domain.Common;

namespace LibraryMS.Domain.BranchManagement.Events;

/// <summary>Raised when a new library branch is created.</summary>
public sealed record BranchCreatedEvent(Guid BranchId, string Name) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
