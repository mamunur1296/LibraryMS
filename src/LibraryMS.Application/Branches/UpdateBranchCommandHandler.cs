using LibraryMS.Domain.Shared;
using LibraryMS.Application.Mapping;
using LibraryMS.Application.Contracts.Branches;
using LibraryMS.Application.Contracts.DTOs.Branch;
using LibraryMS.Domain.BranchManagement;
using LibraryMS.Domain.Shared.Guards;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryMS.Application.Branches;

public sealed class UpdateBranchCommandHandler : IRequestHandler<UpdateBranchCommand, BranchDto>
{
    private readonly BranchManager _manager;
    private readonly IBranchRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateBranchCommandHandler> _logger;

    public UpdateBranchCommandHandler(
        BranchManager manager, IBranchRepository repository,
        IUnitOfWork unitOfWork, ILogger<UpdateBranchCommandHandler> logger)
    {
        _manager = manager; _repository = repository;
        _unitOfWork = unitOfWork; _logger = logger;
    }

    public async Task<BranchDto> Handle(UpdateBranchCommand request, CancellationToken cancellationToken)
    {
        var branch = await _repository.GetByIdAsync(request.Id, cancellationToken);
        Ensure.Found(branch, $"Branch with ID '{request.Id}' was not found.");

        await _manager.UpdateAsync(branch!, request.Name, request.Address, request.Phone, request.Email, cancellationToken);
        await _repository.UpdateAsync(branch!, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Branch '{Name}' (ID: {Id}) updated successfully", branch!.Name, branch.Id);

        return branch.ToDto();
    }
}
