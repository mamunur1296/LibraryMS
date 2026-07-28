
using LibraryMS.Application.Contracts.Auth;
using LibraryMS.Application.Contracts.DTOs.Auth;
using LibraryMS.Domain.IdentityManagement;
using LibraryMS.Domain.Shared.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Auth;

/// <summary>Handles login — validates credentials, issues JWT + refresh token.</summary>
public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtService;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtService,
        ILogger<LoginCommandHandler> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _logger = logger;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Support login with either username or email
        var user = await _userRepository.GetByUsernameAsync(request.Username, cancellationToken)
                   ?? await _userRepository.GetByEmailAsync(request.Username, cancellationToken);

        if (user is null || !user.IsActive)
            throw new UnauthorizedException("Invalid username or password.");

        var isValid = _passwordHasher.Verify(request.Password, user.PasswordHash, user.PasswordSalt);
        if (!isValid)
            throw new UnauthorizedException("Invalid username or password.");

        // Record login time
        var updateMethod = typeof(User).GetMethod("RecordLogin",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        updateMethod?.Invoke(user, null);
        await _userRepository.UpdateAsync(user, cancellationToken);

        // Generate tokens
        var (accessToken, expiresAt) = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken(
            Guid.NewGuid(), user.Id, refreshToken,
            DateTime.UtcNow.AddDays(7), null);

        await _userRepository.AddRefreshTokenAsync(refreshTokenEntity, cancellationToken);

        _logger.LogInformation("User {Username} logged in successfully", user.Username);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt,
            User = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role.ToString(),
                MemberId = user.MemberId
            }
        };
    }
}

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
