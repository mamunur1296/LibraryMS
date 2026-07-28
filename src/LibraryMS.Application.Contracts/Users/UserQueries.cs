using LibraryMS.Application.Contracts.DTOs.Auth;
using MediatR;
using System;
using System.Collections.Generic;

namespace LibraryMS.Application.Contracts.Users;

// ──── Queries ────
public sealed record GetCurrentUserQuery(Guid UserId)
    : IRequest<UserDto?>;

public sealed record GetAllUsersQuery()
    : IRequest<List<UserDto>>;
