using LibraryMS.Application.Contracts.DTOs.Auth;
using LibraryMS.Application.Contracts.Users;
using LibraryMS.Application.Mapping;
using LibraryMS.Domain.BranchManagement;
using LibraryMS.Domain.IdentityManagement;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Users;

public sealed class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, List<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IBranchRepository _branchRepository;
    private readonly ILogger<GetAllUsersQueryHandler> _logger;

    public GetAllUsersQueryHandler(
        IUserRepository userRepository,
        IBranchRepository branchRepository,
        ILogger<GetAllUsersQueryHandler> logger)
    {
        _userRepository = userRepository;
        _branchRepository = branchRepository;
        _logger = logger;
    }

    public async Task<List<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving all users from database.");

        var users = await _userRepository.GetAllUsersAsync(cancellationToken);
        var branches = await _branchRepository.GetAllAsync(cancellationToken);

        _logger.LogInformation("Successfully retrieved {Count} users.", users.Count);

        return users.Select(u =>
        {
            var branch = u.BranchId.HasValue ? branches.FirstOrDefault(b => b.Id == u.BranchId.Value) : null;
            return u.ToDto(branch);
        }).ToList();
    }
}

