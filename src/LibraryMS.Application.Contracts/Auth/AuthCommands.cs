using LibraryMS.Application.Contracts.Common;
using LibraryMS.Application.Contracts.DTOs.Auth;
using MediatR;

namespace LibraryMS.Application.Contracts.Auth;

// ──── Commands ────
public sealed record LoginCommand(string Username, string Password)
    : IRequest<AuthResponse>;

public sealed record RefreshTokenCommand(string AccessToken, string RefreshToken)
    : IRequest<AuthResponse>;

public sealed record RevokeTokenCommand(string RefreshToken)
    : IRequest;

public sealed record RegisterUserCommand(string Username, string Email, string Password, string Role)
    : IRequest<Guid>;
