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

/// <summary>Overdue borrows report row.</summary>
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

/// <summary>Most popular books report.</summary>
public sealed class PopularBookDto
{
    public Guid BookId { get; init; }
    public string Title { get; init; } = default!;
    public string AuthorName { get; init; } = default!;
    public string CategoryName { get; init; } = default!;
    public int TotalBorrows { get; init; }
}

/// <summary>Member activity report row.</summary>
public sealed class MemberActivityDto
{
    public Guid MemberId { get; init; }
    public string FullName { get; init; } = default!;
    public string MembershipNumber { get; init; } = default!;
    public int TotalBorrows { get; init; }
    public int ActiveBorrows { get; init; }
    public int OverdueBorrows { get; init; }
    public decimal TotalFinesPaid { get; init; }
}

/// <summary>Request for report date range.</summary>
public sealed class ReportRequest
{
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public Guid? BranchId { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 50;
}
