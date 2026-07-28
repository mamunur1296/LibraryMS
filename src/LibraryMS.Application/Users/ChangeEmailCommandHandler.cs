using LibraryMS.Application.Contracts.Users;
using LibraryMS.Domain.IdentityManagement;
using LibraryMS.Domain.Shared.Exceptions;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryMS.Application.Users;

public sealed class ChangeEmailCommandHandler : IRequestHandler<ChangeEmailCommand>
{
    private readonly IUserRepository _userRepository;

    public ChangeEmailCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Handle(ChangeEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException("User not found.");

        if (await _userRepository.EmailExistsAsync(request.NewEmail, cancellationToken))
            throw new DomainException("Email is already taken.", "USER_EMAIL_TAKEN");

        user.ChangeEmail(request.NewEmail);

        await _userRepository.UpdateAsync(user, cancellationToken);
    }
}
