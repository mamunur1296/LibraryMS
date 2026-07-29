using LibraryMS.Domain.Common;
using System;

namespace LibraryMS.Domain.MemberManagement.Events;

public sealed record MemberRegisteredEvent(
    Guid MemberId, string FirstName, string LastName, string Email) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
