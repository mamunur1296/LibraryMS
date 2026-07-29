using LibraryMS.Application.Contracts.Users;
using LibraryMS.Domain.IdentityManagement;
using LibraryMS.Domain.Shared;
using LibraryMS.Domain.Shared.Guards;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Users;

public sealed class ChangeEmailCommandHandler : IRequestHandler<ChangeEmailCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ChangeEmailCommandHandler> _logger;

    public ChangeEmailCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ILogger<ChangeEmailCommandHandler> logger)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(ChangeEmailCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Attempting to change email for User {UserId} to {NewEmail}", request.UserId, request.NewEmail);

        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        Ensure.Found(user, "User not found.");

        var emailExists = await _userRepository.EmailExistsAsync(request.NewEmail, cancellationToken);
        Ensure.Against(emailExists, "Email is already taken.", "USER_EMAIL_TAKEN");

        user!.ChangeEmail(request.NewEmail);
        await _userRepository.UpdateAsync(user, cancellationToken);

        var dbFailed = false;
        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save changed email to database for user {UserId}.", request.UserId);
            dbFailed = true;
        }

        Ensure.Against(dbFailed, "An error occurred while updating the user's email in the database.", "DB_UPDATE_ERROR");

        _logger.LogInformation("Email for User {UserId} successfully changed to {NewEmail}", request.UserId, request.NewEmail);
    }
}
