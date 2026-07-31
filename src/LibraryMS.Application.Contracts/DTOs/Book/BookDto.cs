namespace LibraryMS.Application.Contracts.DTOs.Book;

public sealed class BookDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = default!;
    public string ISBN { get; init; } = default!;
    public string? Description { get; init; }
    public int PublicationYear { get; init; }
    public string Language { get; init; } = default!;
    public string? CoverImageUrl { get; init; }
    public string CategoryName { get; init; } = default!;
    public Guid CategoryId { get; init; }
    public string AuthorName { get; init; } = default!;
    public Guid AuthorId { get; init; }
    public int TotalCopies { get; init; }
    public int AvailableCopies { get; init; }
    public DateTime CreatedAt { get; init; }
}
