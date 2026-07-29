namespace LibraryMS.Application.Contracts.DTOs.Report;

public sealed class OverdueReportDto
{
    public Guid BorrowId { get; init; }
    public string MemberName { get; init; } = default!;
    public string MembershipNumber { get; init; } = default!;
    public string MemberEmail { get; init; } = default!;
    public string BookTitle { get; init; } = default!;
    public string BranchName { get; init; } = default!;
    public DateTime BorrowDate { get; init; }
    public DateTime DueDate { get; init; }
    public int OverdueDays { get; init; }
    public decimal AccruedFine { get; init; }
}
