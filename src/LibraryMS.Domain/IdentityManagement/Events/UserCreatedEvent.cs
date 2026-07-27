using LibraryMS.Domain.Common;
using LibraryMS.Domain.Shared.Enums;

namespace LibraryMS.Domain.IdentityManagement.Events;

public sealed record UserCreatedEvent(
    Guid UserId, string Username, string Email, UserRole Role) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
