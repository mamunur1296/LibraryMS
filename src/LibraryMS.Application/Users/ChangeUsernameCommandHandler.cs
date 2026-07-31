using LibraryMS.Application.Contracts.Users;
using LibraryMS.Domain.IdentityManagement;
using LibraryMS.Domain.IdentityManagement.AggregateRoots;
using LibraryMS.Domain.IdentityManagement.Entities;
using LibraryMS.Domain.IdentityManagement.Services;
using LibraryMS.Domain.Shared;
using LibraryMS.Domain.Shared.Guards;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Users;

public sealed class ChangeUsernameCommandHandler : IRequestHandler<ChangeUsernameCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ChangeUsernameCommandHandler> _logger;

    public ChangeUsernameCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ILogger<ChangeUsernameCommandHandler> logger)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(ChangeUsernameCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Attempting to change username for User {UserId} to '{NewUsername}'", request.UserId, request.NewUsername);

        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        Ensure.Found(user, "User not found.");

        var usernameExists = await _userRepository.UsernameExistsAsync(request.NewUsername, cancellationToken);
        Ensure.Against(usernameExists, "Username is already taken.", "USER_USERNAME_TAKEN");

        user!.ChangeUsername(request.NewUsername);
        await _userRepository.UpdateAsync(user, cancellationToken);

        var dbFailed = false;
        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save changed username in database for user {UserId}.", request.UserId);
            dbFailed = true;
        }

        Ensure.Against(dbFailed, "An error occurred while updating the user's username in the database.", "DB_UPDATE_ERROR");

        _logger.LogInformation("Username for User {UserId} successfully changed to '{NewUsername}'", request.UserId, request.NewUsername);
    }
}

