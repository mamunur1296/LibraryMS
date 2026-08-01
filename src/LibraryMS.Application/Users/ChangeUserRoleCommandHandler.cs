using LibraryMS.Application.Contracts.Users;
using LibraryMS.Domain.IdentityManagement;
using LibraryMS.Domain.Shared;
using LibraryMS.Domain.Shared.Enums;
using LibraryMS.Domain.Shared.Guards;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Users;

public sealed class ChangeUserRoleCommandHandler : IRequestHandler<ChangeUserRoleCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ChangeUserRoleCommandHandler> _logger;

    public ChangeUserRoleCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ILogger<ChangeUserRoleCommandHandler> logger)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(ChangeUserRoleCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Attempting to change role of TargetUser {TargetUserId} to {NewRole}", request.TargetUserId, request.NewRole);

        var user = await _userRepository.GetByIdAsync(request.TargetUserId, cancellationToken);
        Ensure.Found(user, "User not found.");

        var parsed = Enum.TryParse<UserRole>(request.NewRole, true, out var role);
        Ensure.Against(!parsed, "Invalid role specified.", "INVALID_ROLE");

        user!.ChangeRole(role);
        await _userRepository.UpdateAsync(user, cancellationToken);

        var dbFailed = false;
        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save changed role in database for user {TargetUserId}.", request.TargetUserId);
            dbFailed = true;
        }

        Ensure.Against(dbFailed, "An error occurred while updating the user's role in the database.", "DB_UPDATE_ERROR");

        _logger.LogInformation("Role of User {TargetUserId} successfully changed to {NewRole}", request.TargetUserId, request.NewRole);
    }
}

