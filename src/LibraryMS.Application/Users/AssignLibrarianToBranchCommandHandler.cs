using LibraryMS.Application.Contracts.Users;
using LibraryMS.Domain.IdentityManagement;
using LibraryMS.Domain.Shared;
using LibraryMS.Domain.Shared.Enums;
using LibraryMS.Domain.Shared.Guards;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Users;

internal sealed class AssignLibrarianToBranchCommandHandler : IRequestHandler<AssignLibrarianToBranchCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AssignLibrarianToBranchCommandHandler> _logger;

    public AssignLibrarianToBranchCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ILogger<AssignLibrarianToBranchCommandHandler> logger)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(AssignLibrarianToBranchCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Assigning librarian {LibrarianId} to branch {BranchId}", request.LibrarianId, request.BranchId);

        var user = await _userRepository.GetByIdAsync(request.LibrarianId, cancellationToken);
        Ensure.Found(user, "User not found.");

        user.AssignBranch(request.BranchId);

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully assigned librarian {LibrarianId} to branch {BranchId}", request.LibrarianId, request.BranchId);
    }
}
