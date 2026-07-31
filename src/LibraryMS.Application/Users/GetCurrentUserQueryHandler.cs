using LibraryMS.Application.Contracts.DTOs.Auth;
using LibraryMS.Application.Contracts.Users;
using LibraryMS.Application.Mapping;
using LibraryMS.Domain.IdentityManagement;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Users;

public sealed class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UserDto?>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetCurrentUserQueryHandler> _logger;

    public GetCurrentUserQueryHandler(
        IUserRepository userRepository,
        ILogger<GetCurrentUserQueryHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<UserDto?> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving current user from database for ID: {UserId}", request.UserId);

        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("User not found for ID: {UserId}", request.UserId);
            return null;
        }

        _logger.LogInformation("Successfully retrieved user details for ID: {UserId}", request.UserId);
        return user.ToDto();
    }
}
