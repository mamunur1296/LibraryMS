using LibraryMS.Application.Contracts.Members;
using LibraryMS.Application.Contracts.Services;
using LibraryMS.Domain.IdentityManagement;
using LibraryMS.Domain.Shared;
using LibraryMS.Domain.Shared.Guards;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Members;

public sealed class ResetMemberPasswordCommandHandler : IRequestHandler<ResetMemberPasswordCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ResetMemberPasswordCommandHandler> _logger;

    public ResetMemberPasswordCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork,
        ILogger<ResetMemberPasswordCommandHandler> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(ResetMemberPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByMemberIdAsync(request.MemberId, cancellationToken);
        Ensure.Found(user, $"No login account found for Member ID '{request.MemberId}'.");

        var (hash, salt) = _passwordHasher.Hash(request.NewPassword);
        user!.UpdatePassword(hash, salt);

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Password reset successfully for Member ID {MemberId}", request.MemberId);
    }
}
