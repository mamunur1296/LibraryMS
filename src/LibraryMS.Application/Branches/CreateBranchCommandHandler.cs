using LibraryMS.Application.Contracts.Branches;
using LibraryMS.Application.Contracts.DTOs.Branch;
using LibraryMS.Application.Mapping;
using LibraryMS.Domain.BranchManagement;
using LibraryMS.Domain.Shared;
using LibraryMS.Domain.Shared.Guards;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Branches;

public sealed class CreateBranchCommandHandler : IRequestHandler<CreateBranchCommand, BranchDto>
{
    private readonly BranchManager _manager;
    private readonly IBranchRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateBranchCommandHandler> _logger;

    public CreateBranchCommandHandler(
        BranchManager manager,
        IBranchRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<CreateBranchCommandHandler> logger)
    {
        _manager = manager;
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<BranchDto> Handle(CreateBranchCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating branch with Name: {Name}, Email: {Email}", request.Name, request.Email);

        var branch = await _manager.CreateAsync(
            request.Name, request.Address, request.Phone, request.Email, cancellationToken);

        var dbFailed = false;
        try
        {
            await _repository.AddAsync(branch, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save branch {Name} to database.", request.Name);
            dbFailed = true;
        }

        Ensure.Against(dbFailed, "An error occurred while saving the branch to the database.", "DB_UPDATE_ERROR");

        _logger.LogInformation("Branch '{Name}' created successfully with ID {Id}", branch.Name, branch.Id);

        return branch.ToDto();
    }
}
