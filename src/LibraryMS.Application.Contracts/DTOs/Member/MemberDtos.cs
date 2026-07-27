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
    public DateTime? SuspendedUntil { get; init; }
    public int ActiveBorrows { get; init; }
}

public sealed class CreateMemberRequest
{
    public string FirstName { get; init; } = default!;
    public string LastName { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string Phone { get; init; } = default!;
    public string? Address { get; init; }
    // Optional: create linked user account
    public string? Username { get; init; }
    public string? Password { get; init; }
}

public sealed class UpdateMemberRequest
{
    public string FirstName { get; init; } = default!;
    public string LastName { get; init; } = default!;
    public string Phone { get; init; } = default!;
    public string? Address { get; init; }
}

public sealed class SuspendMemberRequest
{
    public DateTime SuspendedUntil { get; init; }
    public string Reason { get; init; } = default!;
}

public sealed class MemberSearchRequest
{
    public string? SearchTerm { get; init; }
    public string? Status { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
