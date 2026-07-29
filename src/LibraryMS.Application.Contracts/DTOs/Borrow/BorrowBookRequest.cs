namespace LibraryMS.Application.Contracts.DTOs.Borrow;

public sealed class BorrowBookRequest
{
    public Guid MemberId { get; init; }
    public Guid BookCopyId { get; init; }
    public Guid BookId { get; init; }
    public Guid BranchId { get; init; }
    public int? BorrowDays { get; init; }  // null = use default (14 days)
}
