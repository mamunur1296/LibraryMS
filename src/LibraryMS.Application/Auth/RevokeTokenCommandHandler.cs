using LibraryMS.Application.Contracts.Auth;
using LibraryMS.Domain.IdentityManagement;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Auth;

public sealed class RevokeTokenCommandHandler : IRequestHandler<RevokeTokenCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<RevokeTokenCommandHandler> _logger;

    public RevokeTokenCommandHandler(
        IUserRepository userRepository,
        ILogger<RevokeTokenCommandHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Revoking refresh token.");

        var storedToken = await _userRepository.GetRefreshTokenAsync(request.RefreshToken, cancellationToken);
        if (storedToken != null)
        {
            storedToken.Revoke();
            await _userRepository.RevokeRefreshTokenAsync(request.RefreshToken, cancellationToken);
            _logger.LogInformation("Refresh token revoked successfully.");
        }
        else
        {
            _logger.LogWarning("Refresh token not found for revocation.");
        }
    }
}
