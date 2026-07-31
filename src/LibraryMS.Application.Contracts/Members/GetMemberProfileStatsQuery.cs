using LibraryMS.Application.Contracts.DTOs.Member;
using MediatR;
using System;

namespace LibraryMS.Application.Contracts.Members;

public sealed record GetMemberProfileStatsQuery(Guid MemberId) : IRequest<MemberProfileStatsDto>;
