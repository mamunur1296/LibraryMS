using LibraryMS.Application.Contracts.Users;
using LibraryMS.Domain.IdentityManagement;
using LibraryMS.Domain.IdentityManagement.AggregateRoots;
using LibraryMS.Domain.Shared;
using LibraryMS.Domain.Shared.Guards;
using LibraryMS.Domain.BranchManagement;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Users;

public sealed class AssignBranchToLibrarianCommandHandler : IRequestHandler<AssignBranchToLibrarianCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IBranchRepository _branchRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AssignBranchToLibrarianCommandHandler> _logger;

    public AssignBranchToLibrarianCommandHandler(
        IUserRepository userRepository,
        IBranchRepository branchRepository,
        IUnitOfWork unitOfWork,
        ILogger<AssignBranchToLibrarianCommandHandler> logger)
    {
        _userRepository = userRepository;
        _branchRepository = branchRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(AssignBranchToLibrarianCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Attempting to assign branch {BranchId} to librarian {UserId}", request.BranchId, request.UserId);

        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        Ensure.Found(user, "User not found");

        var branch = await _branchRepository.GetByIdAsync(request.BranchId, cancellationToken);
        Ensure.Found(branch, "Branch not found");

        user.AssignBranch(request.BranchId);
        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully assigned branch {BranchId} to librarian {UserId}", branch.Id, user.Id);
    }
}
