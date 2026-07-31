namespace LibraryMS.Application.Contracts.DTOs.Book;

public sealed class BookSearchRequest
{
    public string? SearchTerm { get; init; }
    public Guid? CategoryId { get; init; }
    public Guid? AuthorId { get; init; }
    public Guid? BranchId { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
