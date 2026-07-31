using LibraryMS.Domain.Common;

namespace LibraryMS.Domain.MemberManagement.Events;

public sealed record MembershipRenewedEvent(Guid MemberId, DateTime NewExpiryDate) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
