using LibraryMS.Application.Contracts.Branches;
using LibraryMS.Domain.BranchManagement;
using LibraryMS.Domain.BranchManagement.AggregateRoots;
using LibraryMS.Domain.BranchManagement.Services;
using LibraryMS.Domain.Shared;
using LibraryMS.Domain.Shared.Guards;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Branches;

public sealed class DeleteBranchCommandHandler : IRequestHandler<DeleteBranchCommand>
{
    private readonly IBranchRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteBranchCommandHandler> _logger;

    public DeleteBranchCommandHandler(
        IBranchRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteBranchCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(DeleteBranchCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting branch with ID: {Id}", request.Id);

        var branch = await _repository.GetByIdAsync(request.Id, cancellationToken);
        Ensure.Found(branch, $"Branch with ID '{request.Id}' was not found.");

        var dbFailed = false;
        try
        {
            await _repository.DeleteAsync(branch!, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete branch with ID {Id} from database.", request.Id);
            dbFailed = true;
        }

        Ensure.Against(dbFailed, "An error occurred while deleting the branch from the database.", "DB_UPDATE_ERROR");

        _logger.LogInformation("Branch with ID {Id} deleted successfully", request.Id);
    }
}

