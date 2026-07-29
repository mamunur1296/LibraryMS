using LibraryMS.Application.Contracts.Auth;
using LibraryMS.Application.Contracts.DTOs.Auth;
using LibraryMS.Application.Mapping;
using LibraryMS.Domain.IdentityManagement;
using LibraryMS.Domain.Shared.Guards;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Auth;

// Handles refresh token rotation.
public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtService;
    private readonly RefreshTokenManager _refreshTokenManager;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;

    public RefreshTokenCommandHandler(
        IUserRepository userRepository,
        IJwtTokenService jwtService,
        RefreshTokenManager refreshTokenManager,
        ILogger<RefreshTokenCommandHandler> logger)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _refreshTokenManager = refreshTokenManager;
        _logger = logger;
    }

    public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing refresh token request.");

        var principal = _jwtService.GetPrincipalFromExpiredToken(request.AccessToken);
        var subClaim = principal.FindFirst("sub")?.Value;
        Ensure.Authorized(subClaim is not null, "Invalid access token.");
        var userId = Guid.Parse(subClaim!);

        var storedToken = await _userRepository.GetRefreshTokenAsync(request.RefreshToken, cancellationToken);
        Ensure.Authorized(storedToken is not null, "Refresh token not found.");
        Ensure.Authorized(storedToken!.IsActive && storedToken.UserId == userId, "Invalid or expired refresh token.");

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        Ensure.Authorized(user is not null, "User not found.");

        // Revoke old token, issue new
        storedToken.Revoke();
        await _userRepository.RevokeRefreshTokenAsync(request.RefreshToken, cancellationToken);

        var (newAccessToken, expiresAt) = _jwtService.GenerateAccessToken(user!);
        var newRefreshToken = _jwtService.GenerateRefreshToken();

        var newTokenEntity = _refreshTokenManager.Create(user!.Id, newRefreshToken, null);
        await _userRepository.AddRefreshTokenAsync(newTokenEntity, cancellationToken);

        _logger.LogInformation("Refresh token rotated successfully for User {UserId} ({Username}).", user.Id, user.Username);

        return new AuthResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = expiresAt,
            User = user.ToDto()
        };
    }
}
