using LibraryMS.Application.Mapping;
using LibraryMS.Application.Contracts.Branches;
using LibraryMS.Application.Contracts.DTOs.Branch;
using LibraryMS.Domain.BranchManagement;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryMS.Application.Branches;

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
