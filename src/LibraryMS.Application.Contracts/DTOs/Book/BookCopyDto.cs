namespace LibraryMS.Application.Contracts.DTOs.Book;

public sealed class BookCopyDto
{
    public Guid Id { get; init; }
    public string CopyNumber { get; init; } = default!;
    public string Status { get; init; } = default!;
    public Guid BranchId { get; init; }
    public string BranchName { get; init; } = default!;
}
