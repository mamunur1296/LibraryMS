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

public sealed class BookCopyDto
{
    public Guid Id { get; init; }
    public string CopyNumber { get; init; } = default!;
    public string Status { get; init; } = default!;
    public Guid BranchId { get; init; }
    public string BranchName { get; init; } = default!;
}

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

public sealed class UpdateBookRequest
{
    public string Title { get; init; } = default!;
    public string? Description { get; init; }
    public int PublicationYear { get; init; }
    public Guid CategoryId { get; init; }
    public Guid AuthorId { get; init; }
    public string Language { get; init; } = "English";
}

public sealed class AddBookCopyRequest
{
    public Guid BranchId { get; init; }
    public int Quantity { get; init; } = 1;
}

public sealed class AuthorDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public string? Biography { get; init; }
}

public sealed class CategoryDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public string? Description { get; init; }
}

public sealed class BookSearchRequest
{
    public string? SearchTerm { get; init; }
    public Guid? CategoryId { get; init; }
    public Guid? AuthorId { get; init; }
    public Guid? BranchId { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
