using LibraryMS.Application.Contracts.Auth;
using LibraryMS.Application.Contracts.Services;
using LibraryMS.Domain.IdentityManagement;
using LibraryMS.Domain.IdentityManagement.Services;
using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.MemberManagement.Services;
using LibraryMS.Domain.Shared;
using LibraryMS.Domain.Shared.Enums;
using LibraryMS.Domain.Shared.Guards;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Auth;

public sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly UserManager _userManager;
    private readonly IMemberRepository _memberRepository;
    private readonly MemberManager _memberManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RegisterUserCommandHandler> _logger;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        UserManager userManager,
        IMemberRepository memberRepository,
        MemberManager memberManager,
        IUnitOfWork unitOfWork,
        ILogger<RegisterUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _userManager = userManager;
        _memberRepository = memberRepository;
        _memberManager = memberManager;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Attempting to register user with Username: {Username}, Email: {Email}", request.Username, request.Email);

        var usernameExists = await _userRepository.UsernameExistsAsync(request.Username, cancellationToken);
        Ensure.Against(usernameExists, "Username is already taken.", "USER_USERNAME_TAKEN");

        var emailExists = await _userRepository.EmailExistsAsync(request.Email, cancellationToken);
        Ensure.Against(emailExists, "Email is already registered.", "USER_EMAIL_TAKEN");

        if (!Enum.TryParse<UserRole>(request.Role, true, out var role))
            role = UserRole.Member; // Default role

        Guid? memberId = null;

        if (role == UserRole.Member)
        {
            var member = await _memberManager.CreateAsync(
                request.FirstName ?? "",
                request.LastName ?? "",
                request.Email,
                request.Phone ?? "",
                address: null,
                cancellationToken);

            await _memberRepository.AddAsync(member, cancellationToken);
            memberId = member.Id;
        }

        var (hash, salt) = _passwordHasher.Hash(request.Password);

        var user = _userManager.Create(request.Username, request.Email, hash, salt, role, memberId);

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {Username} registered successfully with ID {UserId}", user.Username, user.Id);

        return user.Id;
    }
}

