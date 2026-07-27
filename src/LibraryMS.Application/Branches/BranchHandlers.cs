using LibraryMS.Application.Mapping;
using LibraryMS.Application.Contracts.Branches;
using LibraryMS.Application.Contracts.DTOs.Branch;
using LibraryMS.Domain.BranchManagement;
using LibraryMS.Domain.Shared.Exceptions;
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
        BranchManager manager, IBranchRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<CreateBranchCommandHandler> logger)
    {
        _manager = manager; _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<BranchDto> Handle(CreateBranchCommand request, CancellationToken cancellationToken)
    {
        var branch = await _manager.CreateAsync(
            request.Name, request.Address, request.Phone, request.Email, cancellationToken);

        await _repository.AddAsync(branch, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Branch '{Name}' created with ID {Id}", branch.Name, branch.Id);

        return branch.ToDto();
    }
}

public sealed class UpdateBranchCommandHandler : IRequestHandler<UpdateBranchCommand, BranchDto>
{
    private readonly BranchManager _manager;
    private readonly IBranchRepository _repository;
    
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBranchCommandHandler(
        BranchManager manager, IBranchRepository repository,
        IUnitOfWork unitOfWork)
    {
        _manager = manager; _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<BranchDto> Handle(UpdateBranchCommand request, CancellationToken cancellationToken)
    {
        var branch = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Branch), request.Id);

        await _manager.UpdateAsync(branch, request.Name, request.Address, request.Phone, request.Email, cancellationToken);
        await _repository.UpdateAsync(branch, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return branch.ToDto();
    }
}

public sealed class DeleteBranchCommandHandler : IRequestHandler<DeleteBranchCommand>
{
    private readonly IBranchRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBranchCommandHandler(IBranchRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository; _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteBranchCommand request, CancellationToken cancellationToken)
    {
        var branch = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Branch), request.Id);

        await _repository.DeleteAsync(branch, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

public sealed class GetBranchByIdQueryHandler : IRequestHandler<GetBranchByIdQuery, BranchDto?>
{
    private readonly IBranchRepository _repository;
    

    public GetBranchByIdQueryHandler(IBranchRepository repository)
    {
        _repository = repository; 
    }

    public async Task<BranchDto?> Handle(GetBranchByIdQuery request, CancellationToken cancellationToken)
    {
        var branch = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return branch?.ToDto();
    }
}

public sealed class GetAllBranchesQueryHandler : IRequestHandler<GetAllBranchesQuery, List<BranchDto>>
{
    private readonly IBranchRepository _repository;
    

    public GetAllBranchesQueryHandler(IBranchRepository repository)
    {
        _repository = repository; 
    }

    public async Task<List<BranchDto>> Handle(GetAllBranchesQuery request, CancellationToken cancellationToken)
    {
        var branches = await _repository.GetAllAsync(cancellationToken);
        var result = request.IncludeInactive
            ? branches
            : branches.Where(b => b.IsActive).ToList();

        return result.Select(b => b.ToDto()).ToList();
    }
}
