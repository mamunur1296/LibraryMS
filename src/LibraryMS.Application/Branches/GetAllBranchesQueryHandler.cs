using LibraryMS.Application.Contracts.Branches;
using LibraryMS.Application.Contracts.DTOs.Branch;
using LibraryMS.Application.Mapping;
using LibraryMS.Domain.BranchManagement;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Branches;

public sealed class GetAllBranchesQueryHandler : IRequestHandler<GetAllBranchesQuery, List<BranchDto>>
{
    private readonly IBranchRepository _repository;
    private readonly ILogger<GetAllBranchesQueryHandler> _logger;

    public GetAllBranchesQueryHandler(IBranchRepository repository, ILogger<GetAllBranchesQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<List<BranchDto>> Handle(GetAllBranchesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving all branches. IncludeInactive: {IncludeInactive}", request.IncludeInactive);

        var branches = await _repository.GetAllAsync(cancellationToken);
        var result = request.IncludeInactive
            ? branches
            : branches.Where(b => b.IsActive).ToList();

        _logger.LogInformation("Successfully retrieved {Count} branches.", result.Count);

        return result.Select(b => b.ToDto()).ToList();
    }
}
