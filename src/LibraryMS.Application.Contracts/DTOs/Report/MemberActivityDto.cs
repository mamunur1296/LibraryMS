namespace LibraryMS.Application.Contracts.DTOs.Report;

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
