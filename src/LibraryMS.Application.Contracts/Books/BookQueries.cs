using LibraryMS.Application.Contracts.Common;
using LibraryMS.Application.Contracts.DTOs.Book;
using MediatR;

namespace LibraryMS.Application.Contracts.Books;

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

public sealed record GetAvailableCopiesQuery(Guid BookId)
    : IRequest<List<BookCopyDto>>;
