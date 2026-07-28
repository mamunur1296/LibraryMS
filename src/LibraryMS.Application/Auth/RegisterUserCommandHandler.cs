using LibraryMS.Application.Contracts.Auth;
using LibraryMS.Domain.IdentityManagement;
using LibraryMS.Domain.Shared.Enums;
using LibraryMS.Domain.Shared.Exceptions;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryMS.Application.Auth;

public sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        if (await _userRepository.UsernameExistsAsync(request.Username, cancellationToken))
            throw new DomainException("Username is already taken.", "USER_USERNAME_TAKEN");

        if (await _userRepository.EmailExistsAsync(request.Email, cancellationToken))
            throw new DomainException("Email is already registered.", "USER_EMAIL_TAKEN");

        if (!Enum.TryParse<UserRole>(request.Role, true, out var role))
            role = UserRole.Member; // Default role

        var (hash, salt) = _passwordHasher.Hash(request.Password);

        var user = new User(Guid.NewGuid(), request.Username, request.Email, hash, salt, role);

        await _userRepository.AddAsync(user, cancellationToken);

        return user.Id;
    }
}
