namespace LibraryMS.Application.Contracts.DTOs.Report;

public class BranchComparisonDto
{
    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public int TotalBooks { get; set; }
    public int ActiveBorrows { get; set; }
    public int OverdueBorrows { get; set; }
    public decimal TotalRevenue { get; set; }
}
