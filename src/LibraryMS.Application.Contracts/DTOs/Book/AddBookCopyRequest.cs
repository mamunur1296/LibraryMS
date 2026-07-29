namespace LibraryMS.Application.Contracts.DTOs.Book;

public sealed class AddBookCopyRequest
{
    public Guid BranchId { get; init; }
    public int Quantity { get; init; } = 1;
}
