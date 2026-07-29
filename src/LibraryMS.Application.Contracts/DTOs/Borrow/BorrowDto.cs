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
