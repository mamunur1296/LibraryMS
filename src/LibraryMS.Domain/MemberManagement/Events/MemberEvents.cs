using LibraryMS.Domain.Common;

namespace LibraryMS.Domain.MemberManagement.Events;

public sealed record MemberRegisteredEvent(
    Guid MemberId, string FirstName, string LastName, string Email) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public sealed record MemberSuspendedEvent(
    Guid MemberId, string FullName, DateTime UntilDate, string Reason) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
