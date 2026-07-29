using LibraryMS.Application.Contracts.DTOs.Auth;
using LibraryMS.Application.Contracts.Users;
using LibraryMS.Application.Mapping;
using LibraryMS.Domain.IdentityManagement;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Users;

public sealed class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, List<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetAllUsersQueryHandler> _logger;

    public GetAllUsersQueryHandler(IUserRepository userRepository, ILogger<GetAllUsersQueryHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<List<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving all users from database.");

        var users = await _userRepository.GetAllUsersAsync(cancellationToken);

        _logger.LogInformation("Successfully retrieved {Count} users.", users.Count);

        return users.Select(u => u.ToDto()).ToList();
    }
}
