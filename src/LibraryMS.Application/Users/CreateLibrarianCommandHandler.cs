using LibraryMS.Application.Contracts.Users;
using LibraryMS.Application.Contracts.Services;
using LibraryMS.Domain.IdentityManagement;
using LibraryMS.Domain.IdentityManagement.AggregateRoots;
using LibraryMS.Domain.Shared;
using LibraryMS.Domain.Shared.Enums;
using LibraryMS.Domain.Shared.Guards;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Users;

internal sealed class CreateLibrarianCommandHandler : IRequestHandler<CreateLibrarianCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<CreateLibrarianCommandHandler> _logger;

    public CreateLibrarianCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        ILogger<CreateLibrarianCommandHandler> logger)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateLibrarianCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Admin creating librarian with username {Username}", request.Username);

        var existingUser = await _userRepository.GetByUsernameAsync(request.Username, cancellationToken);
        Ensure.Against(existingUser is not null, "Username is already taken.", "USERNAME_TAKEN");

        var existingEmail = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        Ensure.Against(existingEmail is not null, "Email is already registered.", "EMAIL_REGISTERED");

        var (hash, salt) = _passwordHasher.Hash(request.Password);

        var user = new User(
            Guid.NewGuid(),
            request.Username,
            request.Email,
            hash,
            salt,
            UserRole.Librarian,
            null);

        if (request.BranchId.HasValue)
        {
            user.AssignBranch(request.BranchId.Value);
        }

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully created librarian {Username} with ID {UserId}", request.Username, user.Id);

        return user.Id;
    }
}
