using MediatR;

namespace LibraryMS.Application.Contracts.Users;

public sealed record AssignBranchToLibrarianCommand(Guid UserId, Guid BranchId) : IRequest;
