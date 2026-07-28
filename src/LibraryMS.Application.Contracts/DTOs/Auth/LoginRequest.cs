namespace LibraryMS.Application.Contracts.DTOs.Auth;

public sealed class LoginRequest
{
    public string Username { get; init; } = default!;
    public string Password { get; init; } = default!;
}
