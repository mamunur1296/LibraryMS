using LibraryMS.Application.Contracts.Books;
using LibraryMS.Application.Contracts.Common;
using LibraryMS.Application.Contracts.DTOs.Book;
using LibraryMS.Application.Mapping;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.BookManagement.AggregateRoots;
using LibraryMS.Domain.BookManagement.Entities;
using LibraryMS.Domain.BookManagement.Services;
using MediatR;
using LibraryMS.Application.Contracts.Services;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Books;

public sealed class SearchBooksQueryHandler : IRequestHandler<SearchBooksQuery, PagedResult<BookDto>>
{
    private readonly IBookRepository _repository;
    private readonly ICacheService _cache;
    private readonly ILogger<SearchBooksQueryHandler> _logger;

    public SearchBooksQueryHandler(
        IBookRepository repository,
        ICacheService cache,
        ILogger<SearchBooksQueryHandler> logger)
    {
        _repository = repository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<PagedResult<BookDto>> Handle(SearchBooksQuery request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Searching books with Term: '{SearchTerm}', Page: {Page}, PageSize: {PageSize}",
            request.SearchTerm, request.Page, request.PageSize);

        var cacheKey = $"SearchBooks_{request.SearchTerm}_{request.CategoryId}_{request.AuthorId}_{request.BranchId}_{request.Page}_{request.PageSize}";

        var cachedResult = await _cache.GetAsync<PagedResult<BookDto>>(cacheKey, cancellationToken);
        if (cachedResult is not null)
        {
            _logger.LogDebug("Returning cached search results for key: {CacheKey}", cacheKey);
            return cachedResult;
        }

        var (items, total) = await _repository.SearchAsync(
            request.SearchTerm, request.CategoryId, request.AuthorId,
            request.BranchId, request.Page, request.PageSize, cancellationToken);

        _logger.LogDebug("Search books query returned {Count} items out of {Total} total matches.", items.Count, total);

        var authors = await _repository.GetAllAuthorsAsync(cancellationToken);
        var categories = await _repository.GetAllCategoriesAsync(cancellationToken);

        var dtos = items.Select(book => 
        {
            var author = authors.FirstOrDefault(a => a.Id == book.AuthorId);
            var category = categories.FirstOrDefault(c => c.Id == book.CategoryId);
            return book.ToDto(category, author);
        }).ToList();

        var result = PagedResult<BookDto>.Create(dtos, total, request.Page, request.PageSize);
        
        await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5), cancellationToken);

        return result;
    }
}

