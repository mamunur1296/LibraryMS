namespace LibraryMS.Application.Contracts.DTOs.Book;

public sealed class CreateBookRequest
{
    public string Title { get; init; } = default!;
    public string ISBN { get; init; } = default!;
    public string? Description { get; init; }
    public int PublicationYear { get; init; }
    public Guid CategoryId { get; init; }
    public Guid AuthorId { get; init; }
    public string Language { get; init; } = "English";
    public int InitialCopies { get; init; } = 1;
    public Guid BranchId { get; init; }
}
