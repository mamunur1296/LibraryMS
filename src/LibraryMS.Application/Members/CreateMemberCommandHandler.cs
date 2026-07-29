using LibraryMS.Application.Contracts.DTOs.Member;
using LibraryMS.Application.Contracts.Members;
using LibraryMS.Application.Contracts.Services;
using LibraryMS.Application.Mapping;
using LibraryMS.Domain.IdentityManagement;
using LibraryMS.Domain.IdentityManagement.Services;
using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.MemberManagement.AggregateRoots;
using LibraryMS.Domain.MemberManagement.Services;
using LibraryMS.Domain.Shared;
using LibraryMS.Domain.Shared.Enums;
using LibraryMS.Domain.Shared.Guards;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Members;

public sealed class CreateMemberCommandHandler : IRequestHandler<CreateMemberCommand, MemberDto>
{
    private readonly MemberManager _manager;
    private readonly IMemberRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateMemberCommandHandler> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly UserManager _userManager;

    public CreateMemberCommandHandler(
        MemberManager manager,
        IMemberRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<CreateMemberCommandHandler> logger,
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        UserManager userManager)
    {
        _manager = manager;
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _userManager = userManager;
    }

    public async Task<MemberDto> Handle(CreateMemberCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating member: {FirstName} {LastName}, Email: {Email}", request.FirstName, request.LastName, request.Email);

        if (!string.IsNullOrWhiteSpace(request.Username))
        {
            var usernameExists = await _userRepository.UsernameExistsAsync(request.Username, cancellationToken);
            Ensure.Against(usernameExists, "Username is already taken.", "USER_USERNAME_TAKEN");

            var emailExists = await _userRepository.EmailExistsAsync(request.Email, cancellationToken);
            Ensure.Against(emailExists, "Email is already registered as a user.", "USER_EMAIL_TAKEN");
        }

        var member = await _manager.CreateAsync(
            request.FirstName, request.LastName, request.Email,
            request.Phone, request.Address, cancellationToken);

        var dbFailed = false;
        try
        {
            await _repository.AddAsync(member, cancellationToken);

            if (!string.IsNullOrWhiteSpace(request.Username) && !string.IsNullOrWhiteSpace(request.Password))
            {
                var (hash, salt) = _passwordHasher.Hash(request.Password);
                var user = _userManager.Create(request.Username, request.Email, hash, salt, UserRole.Member, member.Id);
                await _userRepository.AddAsync(user, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save member {Name} to database.", member.FullName);
            dbFailed = true;
        }

        Ensure.Against(dbFailed, "An error occurred while saving the member to the database.", "DB_UPDATE_ERROR");

        _logger.LogInformation("Member '{Name}' created successfully with membership number {Number}",
            member.FullName, member.MembershipNumber);

        return member.ToDto();
    }
}

