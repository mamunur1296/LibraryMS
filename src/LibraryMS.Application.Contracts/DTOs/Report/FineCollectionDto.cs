namespace LibraryMS.Application.Contracts.DTOs.Report;

public sealed class FineCollectionDto
{
    public Guid BranchId { get; init; }
    public string BranchName { get; init; } = default!;
    public decimal TotalFinesCollected { get; init; }
    public decimal TotalPendingFines { get; init; }
    public int NumberOfFinesPaid { get; init; }
    public int NumberOfPendingFines { get; init; }
}
