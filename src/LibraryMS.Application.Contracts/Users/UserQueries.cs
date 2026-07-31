using LibraryMS.Application.Contracts.DTOs.Auth;
using MediatR;

namespace LibraryMS.Application.Contracts.Users;

// ──── Queries ────
public sealed record GetCurrentUserQuery(Guid UserId)
    : IRequest<UserDto?>;

public sealed record GetAllUsersQuery()
    : IRequest<List<UserDto>>;
