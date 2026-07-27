namespace LibraryMS.Application.Contracts.DTOs.Borrow;

public sealed class BorrowDto
{
    public Guid Id { get; init; }
    public Guid MemberId { get; init; }
    public string MemberName { get; init; } = default!;
    public string MembershipNumber { get; init; } = default!;
    public Guid BookId { get; init; }
    public string BookTitle { get; init; } = default!;
    public string BookISBN { get; init; } = default!;
    public string CopyNumber { get; init; } = default!;
    public Guid BranchId { get; init; }
    public string BranchName { get; init; } = default!;
    public DateTime BorrowDate { get; init; }
    public DateTime DueDate { get; init; }
    public DateTime? ReturnDate { get; init; }
    public string Status { get; init; } = default!;
    public decimal LateFine { get; init; }
    public bool IsFinePaid { get; init; }
    public bool IsOverdue { get; init; }
    public int DaysUntilDue { get; init; }
}

public sealed class BorrowBookRequest
{
    public Guid MemberId { get; init; }
    public Guid BookCopyId { get; init; }
    public Guid BookId { get; init; }
    public Guid BranchId { get; init; }
    public int? BorrowDays { get; init; }  // null = use default (14 days)
}

public sealed class ReturnBookRequest
{
    public Guid BorrowId { get; init; }
    public string? Notes { get; init; }
}

public sealed class BorrowSearchRequest
{
    public Guid? MemberId { get; init; }
    public Guid? BookId { get; init; }
    public string? Status { get; init; }  // "Active", "Returned", "Overdue"
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
