using LibraryMS.Application.Contracts.DTOs.Book;
using MediatR;

namespace LibraryMS.Application.Contracts.Books;

// ──── Commands ────
public sealed record CreateBookCommand(
    string Title, string ISBN, string? Description,
    int PublicationYear, Guid CategoryId, Guid AuthorId,
    string Language, int InitialCopies, Guid BranchId)
    : IRequest<BookDto>;

public sealed record UpdateBookCommand(
    Guid Id, string Title, string? Description,
    int PublicationYear, Guid CategoryId, Guid AuthorId, string Language)
    : IRequest<BookDto>;

public sealed record DeleteBookCommand(Guid Id)
    : IRequest;

public sealed record AddBookCopiesCommand(Guid BookId, Guid BranchId, int Quantity)
    : IRequest<List<BookCopyDto>>;

public sealed record CreateAuthorCommand(string Name, string? Biography)
    : IRequest<AuthorDto>;

public sealed record CreateCategoryCommand(string Name, string? Description)
    : IRequest<CategoryDto>;
