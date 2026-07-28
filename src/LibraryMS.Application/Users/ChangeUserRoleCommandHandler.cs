using LibraryMS.Application.Contracts.Auth;
using LibraryMS.Application.Contracts.Users;
using LibraryMS.Domain.IdentityManagement;
using LibraryMS.Domain.Shared.Enums;
using LibraryMS.Domain.Shared.Exceptions;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryMS.Application.Users;

public sealed class ChangeUserRoleCommandHandler : IRequestHandler<ChangeUserRoleCommand>
{
    private readonly IUserRepository _userRepository;

    public ChangeUserRoleCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Handle(ChangeUserRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.TargetUserId, cancellationToken)
            ?? throw new NotFoundException("User not found.");

        if (!Enum.TryParse<UserRole>(request.NewRole, true, out var role))
            throw new DomainException("Invalid role specified.", "INVALID_ROLE");

        user.ChangeRole(role);

        await _userRepository.UpdateAsync(user, cancellationToken);
    }
}
