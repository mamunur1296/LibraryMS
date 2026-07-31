using System;
using System.Collections.Generic;
using LibraryMS.Application.Contracts.DTOs.Member;
using MediatR;

namespace LibraryMS.Application.Contracts.Members;

public record GetMemberFavoritesQuery(Guid MemberId) : IRequest<List<MemberFavoriteDto>>;
