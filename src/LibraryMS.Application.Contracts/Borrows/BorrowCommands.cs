using LibraryMS.Application.Contracts.Common;
using LibraryMS.Application.Contracts.DTOs.Borrow;
using MediatR;

namespace LibraryMS.Application.Contracts.Borrows;

// ──── Commands ────
public sealed record BorrowBookCommand(
    Guid MemberId, Guid BookCopyId, Guid BookId,
    Guid BranchId, int? BorrowDays = null)
    : IRequest<BorrowDto>, IRetryableRequest;

public sealed record ReturnBookCommand(Guid BorrowId, string? Notes = null)
    : IRequest<BorrowDto>, IRetryableRequest;

public sealed record PayFineCommand(Guid BorrowId)
    : IRequest<BorrowDto>, IRetryableRequest;
