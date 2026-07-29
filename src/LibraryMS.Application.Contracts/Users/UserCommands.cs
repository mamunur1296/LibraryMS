using MediatR;

namespace LibraryMS.Application.Contracts.Users;

// ──── Commands ────
public sealed record ChangePasswordCommand(Guid UserId, string CurrentPassword, string NewPassword)
    : IRequest;

public sealed record ChangeUsernameCommand(Guid UserId, string NewUsername)
    : IRequest;

public sealed record ChangeEmailCommand(Guid UserId, string NewEmail)
    : IRequest;

public sealed record ChangeUserRoleCommand(Guid TargetUserId, string NewRole)
    : IRequest;
