namespace LibraryMS.Application.Contracts.DTOs.Auth;

public sealed class LoginRequest
{
    public string Username { get; init; } = default!;
    public string Password { get; init; } = default!;
}

public sealed class RefreshTokenRequest
{
    public string AccessToken { get; init; } = default!;
    public string RefreshToken { get; init; } = default!;
}

public sealed class AuthResponse
{
    public string AccessToken { get; init; } = default!;
    public string RefreshToken { get; init; } = default!;
    public DateTime ExpiresAt { get; init; }
    public UserDto User { get; init; } = default!;
}

public sealed class UserDto
{
    public Guid Id { get; init; }
    public string Username { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string Role { get; init; } = default!;
    public Guid? MemberId { get; init; }
}
