using LibraryMS.Application.Contracts.Common;
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

// ──── Queries ────
public sealed record GetBookByIdQuery(Guid Id)
    : IRequest<BookDto?>;

public sealed record SearchBooksQuery(
    string? SearchTerm, Guid? CategoryId, Guid? AuthorId,
    Guid? BranchId, int Page, int PageSize)
    : IRequest<PagedResult<BookDto>>;

public sealed record GetAllAuthorsQuery()
    : IRequest<List<AuthorDto>>;

public sealed record GetAllCategoriesQuery()
    : IRequest<List<CategoryDto>>;
