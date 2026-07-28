namespace LibraryMS.Application.Contracts.DTOs.Auth;

public sealed class RefreshTokenRequest
{
    public string AccessToken { get; init; } = default!;
    public string RefreshToken { get; init; } = default!;
}
