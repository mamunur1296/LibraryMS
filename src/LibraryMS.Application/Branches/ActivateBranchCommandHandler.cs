using LibraryMS.Application.Contracts.Branches;
using LibraryMS.Application.Contracts.DTOs.Branch;
using LibraryMS.Application.Mapping;
using LibraryMS.Domain.BranchManagement;
using LibraryMS.Domain.Shared;
using LibraryMS.Domain.Shared.Guards;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Branches;

public sealed class ActivateBranchCommandHandler : IRequestHandler<ActivateBranchCommand, BranchDto>
{
    private readonly IBranchRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ActivateBranchCommandHandler> _logger;

    public ActivateBranchCommandHandler(
        IBranchRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<ActivateBranchCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<BranchDto> Handle(ActivateBranchCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Activating branch with ID: {Id}", request.Id);

        var branch = await _repository.GetByIdAsync(request.Id, cancellationToken);
        Ensure.Found(branch, $"Branch with ID '{request.Id}' was not found.");

        branch!.Activate();
        await _repository.UpdateAsync(branch, cancellationToken);

        var dbFailed = false;
        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to activate branch with ID {Id} in database.", request.Id);
            dbFailed = true;
        }

        Ensure.Against(dbFailed, "An error occurred while activating the branch in the database.", "DB_UPDATE_ERROR");

        _logger.LogInformation("Branch '{Name}' activated successfully.", branch.Name);

        return branch.ToDto();
    }
}
