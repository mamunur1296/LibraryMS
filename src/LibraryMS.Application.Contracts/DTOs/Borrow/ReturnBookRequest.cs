namespace LibraryMS.Application.Contracts.DTOs.Borrow;

public sealed class ReturnBookRequest
{
    public Guid BorrowId { get; init; }
    public string? Notes { get; init; }
}
