namespace LibraryMS.Application.Contracts.DTOs.Member;

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
