using LibraryMS.Application.Contracts.Common;
using LibraryMS.Application.Contracts.DTOs.Member;
using MediatR;

namespace LibraryMS.Application.Contracts.Members;

// ──── Queries ────
public sealed record GetMemberByIdQuery(Guid Id)
    : IRequest<MemberDto?>;

public sealed record SearchMembersQuery(
    string? SearchTerm, string? Status, int Page, int PageSize)
    : IRequest<PagedResult<MemberDto>>;
public sealed record GetMemberProfileStatsQuery(Guid MemberId) : IRequest<MemberProfileStatsDto>;