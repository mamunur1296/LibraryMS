using LibraryMS.Application.Contracts.Auth;
using LibraryMS.Application.Contracts.DTOs.Auth;
using LibraryMS.Domain.IdentityManagement;
using LibraryMS.Domain.Shared.Exceptions;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryMS.Application.Auth;

/// <summary>Handles refresh token rotation.</summary>
public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtService;

    public RefreshTokenCommandHandler(IUserRepository userRepository, IJwtTokenService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var principal = _jwtService.GetPrincipalFromExpiredToken(request.AccessToken);
        var userId = Guid.Parse(principal.FindFirst("sub")?.Value
            ?? throw new UnauthorizedException("Invalid access token."));

        var storedToken = await _userRepository.GetRefreshTokenAsync(request.RefreshToken, cancellationToken)
            ?? throw new UnauthorizedException("Refresh token not found.");

        if (!storedToken.IsActive || storedToken.UserId != userId)
            throw new UnauthorizedException("Invalid or expired refresh token.");

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new UnauthorizedException("User not found.");

        // Revoke old token, issue new
        storedToken.Revoke();
        await _userRepository.RevokeRefreshTokenAsync(request.RefreshToken, cancellationToken);

        var (newAccessToken, expiresAt) = _jwtService.GenerateAccessToken(user);
        var newRefreshToken = _jwtService.GenerateRefreshToken();

        var newTokenEntity = new RefreshToken(Guid.NewGuid(), user.Id, newRefreshToken,
            DateTime.UtcNow.AddDays(7), null);
        await _userRepository.AddRefreshTokenAsync(newTokenEntity, cancellationToken);

        return new AuthResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = expiresAt,
            User = new UserDto
            {
                Id = user.Id, Username = user.Username,
                Email = user.Email, Role = user.Role.ToString(),
                MemberId = user.MemberId
            }
        };
    }
}
