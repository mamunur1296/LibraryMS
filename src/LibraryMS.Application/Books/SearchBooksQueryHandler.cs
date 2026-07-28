using LibraryMS.Application.Mapping;
using LibraryMS.Application.Contracts.Books;
using LibraryMS.Application.Contracts.DTOs.Book;
using LibraryMS.Application.Contracts.Common;
using LibraryMS.Domain.BookManagement;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryMS.Application.Books;

public sealed class SearchBooksQueryHandler : IRequestHandler<SearchBooksQuery, PagedResult<BookDto>>
{
    private readonly IBookRepository _repository;

    public SearchBooksQueryHandler(IBookRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<BookDto>> Handle(SearchBooksQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await _repository.SearchAsync(
            request.SearchTerm, request.CategoryId, request.AuthorId,
            request.BranchId, request.Page, request.PageSize, cancellationToken);

        return PagedResult<BookDto>.Create(
            items.Select(i => i.ToDto()).ToList(),
            total, request.Page, request.PageSize);
    }
}
