namespace LibraryMS.Application.Contracts.DTOs.Report;

/// <summary>Dashboard summary cards data.</summary>
public sealed class DashboardSummaryDto
{
    public int TotalBooks { get; init; }
    public int TotalMembers { get; init; }
    public int ActiveBorrows { get; init; }
    public int OverdueBorrows { get; init; }
    public int PendingReservations { get; init; }
    public int TotalBranches { get; init; }
    public decimal TotalLateFinesCollected { get; init; }
    public decimal PendingLateFines { get; init; }
}
