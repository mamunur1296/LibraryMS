namespace LibraryMS.Application.Contracts.DTOs.Report;

public sealed class AdminDashboardSummaryDto
{
    public DashboardSummaryDto TotalSummary { get; init; } = new();
    public List<BranchDashboardSummaryDto> BranchSummaries { get; init; } = new();
}

public sealed class BranchDashboardSummaryDto
{
    public Guid BranchId { get; init; }
    public string BranchName { get; init; } = string.Empty;
    public int TotalBooks { get; init; }
    public int TotalMembers { get; init; }
    public int ActiveBorrows { get; init; }
    public int OverdueBorrows { get; init; }
    public decimal TotalRevenue { get; init; }
}
