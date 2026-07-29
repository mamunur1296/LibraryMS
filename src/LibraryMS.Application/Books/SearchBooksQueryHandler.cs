using LibraryMS.Application.Contracts.Books;
using LibraryMS.Application.Contracts.Common;
using LibraryMS.Application.Contracts.DTOs.Book;
using LibraryMS.Application.Mapping;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.BookManagement.AggregateRoots;
using LibraryMS.Domain.BookManagement.Entities;
using LibraryMS.Domain.BookManagement.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Books;

public sealed class SearchBooksQueryHandler : IRequestHandler<SearchBooksQuery, PagedResult<BookDto>>
{
    private readonly IBookRepository _repository;
    private readonly ILogger<SearchBooksQueryHandler> _logger;

    public SearchBooksQueryHandler(
        IBookRepository repository,
        ILogger<SearchBooksQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<PagedResult<BookDto>> Handle(SearchBooksQuery request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Searching books with Term: '{SearchTerm}', Page: {Page}, PageSize: {PageSize}",
            request.SearchTerm, request.Page, request.PageSize);

        var (items, total) = await _repository.SearchAsync(
            request.SearchTerm, request.CategoryId, request.AuthorId,
            request.BranchId, request.Page, request.PageSize, cancellationToken);

        _logger.LogDebug("Search books query returned {Count} items out of {Total} total matches.", items.Count, total);

        return PagedResult<BookDto>.Create(
            items.Select(i => i.ToDto()).ToList(),
            total, request.Page, request.PageSize);
    }
}

