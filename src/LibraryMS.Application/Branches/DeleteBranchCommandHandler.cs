using LibraryMS.Domain.Shared;
using LibraryMS.Application.Contracts.Branches;
using LibraryMS.Domain.BranchManagement;
using LibraryMS.Domain.Shared.Guards;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryMS.Application.Branches;

public sealed class DeleteBranchCommandHandler : IRequestHandler<DeleteBranchCommand>
{
    private readonly IBranchRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteBranchCommandHandler> _logger;

    public DeleteBranchCommandHandler(
        IBranchRepository repository, IUnitOfWork unitOfWork,
        ILogger<DeleteBranchCommandHandler> logger)
    {
        _repository = repository; _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(DeleteBranchCommand request, CancellationToken cancellationToken)
    {
        var branch = await _repository.GetByIdAsync(request.Id, cancellationToken);
        Ensure.Found(branch, $"Branch with ID '{request.Id}' was not found.");

        await _repository.DeleteAsync(branch!, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Branch with ID {Id} deleted successfully", request.Id);
    }
}
