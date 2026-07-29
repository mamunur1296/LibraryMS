namespace LibraryMS.Application.Contracts.DTOs.Report;

public sealed class ReportRequest
{
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public Guid? BranchId { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 50;
}
