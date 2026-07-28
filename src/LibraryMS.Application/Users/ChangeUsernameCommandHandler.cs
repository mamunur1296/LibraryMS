using LibraryMS.Application.Contracts.Users;
using LibraryMS.Domain.IdentityManagement;
using LibraryMS.Domain.Shared.Exceptions;
using LibraryMS.Domain.Shared.Guards;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryMS.Application.Users;

public sealed class ChangeUsernameCommandHandler : IRequestHandler<ChangeUsernameCommand>
{
    private readonly IUserRepository _userRepository;

    public ChangeUsernameCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Handle(ChangeUsernameCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        Ensure.Found(user, "User not found.");

        var usernameExists = await _userRepository.UsernameExistsAsync(request.NewUsername, cancellationToken);
        Ensure.Against(usernameExists, "Username is already taken.", "USER_USERNAME_TAKEN");

        user.ChangeUsername(request.NewUsername);

        await _userRepository.UpdateAsync(user, cancellationToken);
    }
}
