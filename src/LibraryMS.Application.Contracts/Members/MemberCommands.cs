using LibraryMS.Application.Contracts.DTOs.Member;
using MediatR;

namespace LibraryMS.Application.Contracts.Members;

// ──── Commands ────
public sealed record CreateMemberCommand(
    string FirstName, string LastName, string Email,
    string Phone, string? Address,
    string? Username, string? Password)
    : IRequest<MemberDto>;

public sealed record UpdateMemberCommand(
    Guid Id, string FirstName, string LastName,
    string Phone, string? Address)
    : IRequest<MemberDto>;

public sealed record DeleteMemberCommand(Guid Id)
    : IRequest;

public sealed record SuspendMemberCommand(
    Guid Id, DateTime SuspendedUntil, string Reason)
    : IRequest<MemberDto>;

public sealed record ActivateMemberCommand(Guid Id)
    : IRequest<MemberDto>;

public sealed record ResetMemberPasswordCommand(Guid MemberId, string NewPassword) : IRequest;

public sealed record CreateMemberUserCommand(Guid MemberId, string Username, string Password) : IRequest;

public sealed record RenewMembershipCommand(Guid Id, int Days) : IRequest<MemberDto>;