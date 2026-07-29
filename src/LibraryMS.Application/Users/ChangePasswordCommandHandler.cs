using LibraryMS.Application.Contracts.Auth;
using LibraryMS.Application.Contracts.Users;
using LibraryMS.Domain.IdentityManagement;
using LibraryMS.Domain.Shared;
using LibraryMS.Domain.Shared.Guards;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Users;

public sealed class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ChangePasswordCommandHandler> _logger;

    public ChangePasswordCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork,
        ILogger<ChangePasswordCommandHandler> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Attempting to change password for User {UserId}", request.UserId);

        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        Ensure.Found(user, "User not found.");

        var isAuthorized = _passwordHasher.Verify(request.CurrentPassword, user!.PasswordHash, user.PasswordSalt);
        Ensure.Authorized(isAuthorized, "Incorrect current password.");

        var (newHash, newSalt) = _passwordHasher.Hash(request.NewPassword);
        user.UpdatePassword(newHash, newSalt);

        await _userRepository.UpdateAsync(user, cancellationToken);

        var dbFailed = false;
        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save changed password to database for user {UserId}.", request.UserId);
            dbFailed = true;
        }

        Ensure.Against(dbFailed, "An error occurred while updating the user's password in the database.", "DB_UPDATE_ERROR");

        _logger.LogInformation("Password for User {UserId} successfully changed.", request.UserId);
    }
}
