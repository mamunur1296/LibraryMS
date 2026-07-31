using LibraryMS.Application.Contracts.Common;
using LibraryMS.Application.Contracts.DTOs.Borrow;
using MediatR;

namespace LibraryMS.Application.Contracts.Borrows;

// ──── Queries ────
public sealed record GetBorrowByIdQuery(Guid Id)
    : IRequest<BorrowDto?>;

public sealed record GetBorrowsQuery(
    Guid? MemberId, Guid? BookId, string? Status, int Page, int PageSize)
    : IRequest<PagedResult<BorrowDto>>;

public sealed record GetActiveBorrowsByMemberQuery(Guid MemberId)
    : IRequest<List<BorrowDto>>;

public sealed record GetOverdueBorrowsQuery(int Page, int PageSize)
    : IRequest<PagedResult<BorrowDto>>;
