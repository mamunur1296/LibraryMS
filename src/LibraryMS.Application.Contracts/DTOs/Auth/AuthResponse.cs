namespace LibraryMS.Application.Contracts.DTOs.Auth;

public sealed class AuthResponse
{
    public string AccessToken { get; init; } = default!;
    public string RefreshToken { get; init; } = default!;
    public DateTime ExpiresAt { get; init; }
    public UserDto User { get; init; } = default!;
}
