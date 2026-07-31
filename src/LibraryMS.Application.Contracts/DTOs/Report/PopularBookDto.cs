namespace LibraryMS.Application.Contracts.DTOs.Report;

public sealed class PopularBookDto
{
    public Guid BookId { get; init; }
    public string Title { get; init; } = default!;
    public string AuthorName { get; init; } = default!;
    public string CategoryName { get; init; } = default!;
    public int TotalBorrows { get; init; }
}
