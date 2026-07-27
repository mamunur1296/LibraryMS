using LibraryMS.Application.Contracts.Common;
using LibraryMS.Application.Contracts.DTOs.Branch;
using MediatR;

namespace LibraryMS.Application.Contracts.Branches;

// ──── Commands ────
public sealed record CreateBranchCommand(
    string Name, string Address, string Phone, string Email)
    : IRequest<BranchDto>;

public sealed record UpdateBranchCommand(
    Guid Id, string Name, string Address, string Phone, string Email)
    : IRequest<BranchDto>;

public sealed record DeleteBranchCommand(Guid Id)
    : IRequest;

public sealed record ActivateBranchCommand(Guid Id)
    : IRequest<BranchDto>;

public sealed record DeactivateBranchCommand(Guid Id)
    : IRequest<BranchDto>;

// ──── Queries ────
public sealed record GetBranchByIdQuery(Guid Id)
    : IRequest<BranchDto?>;

public sealed record GetAllBranchesQuery(bool IncludeInactive = false)
    : IRequest<List<BranchDto>>;
