namespace LibraryMS.Application.Contracts.DTOs.Auth;

public sealed class UserDto
{
    public Guid Id { get; init; }
    public string Username { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string Role { get; init; } = default!;
    public bool IsActive { get; init; }
    public Guid? MemberId { get; init; }
    public Guid? BranchId { get; init; }
    public string? BranchName { get; init; }
}
