using LibraryMS.Application.Contracts.Auth;
using LibraryMS.Application.Contracts.DTOs.Auth;
using LibraryMS.Application.Contracts.Services;
using LibraryMS.Application.Mapping;
using LibraryMS.Domain.BranchManagement;
using LibraryMS.Domain.IdentityManagement;
using LibraryMS.Domain.IdentityManagement.Services;
using LibraryMS.Domain.Shared.Guards;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Auth;

/// Handles login — validates credentials, issues JWT + refresh token
public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IBranchRepository _branchRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtService;
    private readonly ILogger<LoginCommandHandler> _logger;
    private readonly UserManager _userManager;
    private readonly RefreshTokenManager _refreshTokenManager;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IBranchRepository branchRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtService,
        ILogger<LoginCommandHandler> logger,
        UserManager userManager,
        RefreshTokenManager refreshTokenManager)
    {
        _userRepository = userRepository;
        _branchRepository = branchRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _logger = logger;
        _userManager = userManager;
        _refreshTokenManager = refreshTokenManager;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Attempting login for identifier: {UsernameOrEmail}", request.Username);

        // Support login with either username or email
        var user = await _userRepository.GetByUsernameAsync(request.Username, cancellationToken)
                   ?? await _userRepository.GetByEmailAsync(request.Username, cancellationToken);

        Ensure.Authorized(user is not null && user.IsActive);

        var isValid = _passwordHasher.Verify(request.Password, user!.PasswordHash, user.PasswordSalt);
        Ensure.Authorized(isValid);

        // Record login time using the domain service
        _userManager.RecordLogin(user);
        await _userRepository.UpdateAsync(user, cancellationToken);

        // Generate tokens
        var (accessToken, expiresAt) = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        var refreshTokenEntity = _refreshTokenManager.Create(user.Id, refreshToken, null);

        await _userRepository.AddRefreshTokenAsync(refreshTokenEntity, cancellationToken);

        _logger.LogInformation("User {Username} logged in successfully.", user.Username);

        LibraryMS.Domain.BranchManagement.AggregateRoots.Branch? branch = null;
        if (user.BranchId.HasValue)
        {
            branch = await _branchRepository.GetByIdAsync(user.BranchId.Value, cancellationToken);
        }

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt,
            User = user.ToDto(branch)
        };
    }
}

