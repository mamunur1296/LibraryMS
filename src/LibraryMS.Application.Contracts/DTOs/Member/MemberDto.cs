namespace LibraryMS.Application.Contracts.DTOs.Member;

public sealed class MemberDto
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = default!;
    public string LastName { get; init; } = default!;
    public string FullName { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string Phone { get; init; } = default!;
    public string MembershipNumber { get; init; } = default!;
    public string? Address { get; init; }
    public string Status { get; init; } = default!;
    public DateTime JoinDate { get; init; }
    public DateTime MembershipExpiry { get; init; }
    public DateTime? SuspendedUntil { get; init; }
    public int ActiveBorrows { get; init; }
    public bool HasAccount { get; init; }
}
