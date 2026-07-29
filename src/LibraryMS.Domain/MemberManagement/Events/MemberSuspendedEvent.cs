using LibraryMS.Domain.Common;

namespace LibraryMS.Domain.MemberManagement.Events;

public sealed record MemberSuspendedEvent(
    Guid MemberId, string FullName, DateTime UntilDate, string Reason) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
