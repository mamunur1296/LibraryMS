using LibraryMS.Application.Contracts.DTOs.Branch;
using MediatR;

namespace LibraryMS.Application.Contracts.Branches;

// ──── Queries ────
public sealed record GetBranchByIdQuery(Guid Id)
    : IRequest<BranchDto?>;

public sealed record GetAllBranchesQuery(bool IncludeInactive = false)
    : IRequest<List<BranchDto>>;
