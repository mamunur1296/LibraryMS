using LibraryMS.Application.Contracts.Members;
using LibraryMS.Application.Contracts.Services;
using LibraryMS.Domain.IdentityManagement;
using LibraryMS.Domain.IdentityManagement.Services;
using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.Shared;
using LibraryMS.Domain.Shared.Enums;
using LibraryMS.Domain.Shared.Guards;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryMS.Application.Members;

public sealed class CreateMemberUserCommandHandler : IRequestHandler<CreateMemberUserCommand>
{
    private readonly IMemberRepository _memberRepository;
    private readonly IUserRepository _userRepository;
    private readonly UserManager _userManager;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateMemberUserCommandHandler> _logger;

    public CreateMemberUserCommandHandler(
        IMemberRepository memberRepository,
        IUserRepository userRepository,
        UserManager userManager,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork,
        ILogger<CreateMemberUserCommandHandler> logger)
    {
        _memberRepository = memberRepository;
        _userRepository = userRepository;
        _userManager = userManager;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(CreateMemberUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating login account for Member ID: {MemberId}", request.MemberId);

        var member = await _memberRepository.GetByIdAsync(request.MemberId, cancellationToken);
        Ensure.Found(member, $"Member with ID '{request.MemberId}' was not found.");

        var existingUser = await _userRepository.GetByMemberIdAsync(request.MemberId, cancellationToken);
        Ensure.Against(existingUser != null, "This member already has a login account.", "MEMBER_USER_ALREADY_EXISTS");

        var usernameExists = await _userRepository.UsernameExistsAsync(request.Username, cancellationToken);
        Ensure.Against(usernameExists, "Username is already taken.", "USER_USERNAME_TAKEN");

        var emailExists = await _userRepository.EmailExistsAsync(member!.Email, cancellationToken);
        Ensure.Against(emailExists, "Email is already registered as a user.", "USER_EMAIL_TAKEN");

        var (hash, salt) = _passwordHasher.Hash(request.Password);
        var user = _userManager.Create(request.Username, member!.Email, hash, salt, UserRole.Member, member!.Id);

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Login account created successfully for Member ID {MemberId}", request.MemberId);
    }
}
