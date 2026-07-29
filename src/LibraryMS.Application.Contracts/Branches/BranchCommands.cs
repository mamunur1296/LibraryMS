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
