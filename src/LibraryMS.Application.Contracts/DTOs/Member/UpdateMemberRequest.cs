namespace LibraryMS.Application.Contracts.DTOs.Member;

public sealed class UpdateMemberRequest
{
    public string FirstName { get; init; } = default!;
    public string LastName { get; init; } = default!;
    public string Phone { get; init; } = default!;
    public string? Address { get; init; }
}
