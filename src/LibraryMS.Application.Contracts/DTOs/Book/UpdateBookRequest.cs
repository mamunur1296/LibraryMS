namespace LibraryMS.Application.Contracts.DTOs.Book;

public sealed class UpdateBookRequest
{
    public string Title { get; init; } = default!;
    public string? Description { get; init; }
    public int PublicationYear { get; init; }
    public Guid CategoryId { get; init; }
    public Guid AuthorId { get; init; }
    public string Language { get; init; } = "English";
}
