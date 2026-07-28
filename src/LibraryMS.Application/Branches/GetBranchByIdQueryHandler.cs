using LibraryMS.Application.Mapping;
using LibraryMS.Application.Contracts.Branches;
using LibraryMS.Application.Contracts.DTOs.Branch;
using LibraryMS.Domain.BranchManagement;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryMS.Application.Branches;

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
