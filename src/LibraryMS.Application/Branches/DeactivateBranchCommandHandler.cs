using LibraryMS.Application.Contracts.Branches;
using LibraryMS.Application.Contracts.DTOs.Branch;
using LibraryMS.Application.Mapping;
using LibraryMS.Domain.BranchManagement;
using LibraryMS.Domain.Shared;
using LibraryMS.Domain.Shared.Guards;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Branches;

public sealed class DeactivateBranchCommandHandler : IRequestHandler<DeactivateBranchCommand, BranchDto>
{
    private readonly IBranchRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeactivateBranchCommandHandler> _logger;

    public DeactivateBranchCommandHandler(
        IBranchRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<DeactivateBranchCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<BranchDto> Handle(DeactivateBranchCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deactivating branch with ID: {Id}", request.Id);

        var branch = await _repository.GetByIdAsync(request.Id, cancellationToken);
        Ensure.Found(branch, $"Branch with ID '{request.Id}' was not found.");

        branch!.Deactivate();
        await _repository.UpdateAsync(branch, cancellationToken);

        var dbFailed = false;
        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deactivate branch with ID {Id} in database.", request.Id);
            dbFailed = true;
        }

        Ensure.Against(dbFailed, "An error occurred while deactivating the branch in the database.", "DB_UPDATE_ERROR");

        _logger.LogInformation("Branch '{Name}' deactivated successfully.", branch.Name);

        return branch.ToDto();
    }
}
