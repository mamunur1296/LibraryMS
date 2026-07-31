using LibraryMS.Application.Contracts.Common;
using LibraryMS.Application.Contracts.DTOs.Report;
using LibraryMS.Application.Contracts.Reports;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.BookManagement.AggregateRoots;
using LibraryMS.Domain.BorrowManagement;
using LibraryMS.Domain.BorrowManagement.AggregateRoots;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Reports;

public sealed class GetPopularBooksQueryHandler : IRequestHandler<GetPopularBooksQuery, PagedResult<PopularBookDto>>
{
    private readonly IBorrowRepository _borrowRepo;
    private readonly IBookRepository _bookRepo;
    private readonly ILogger<GetPopularBooksQueryHandler> _logger;

    public GetPopularBooksQueryHandler(
        IBorrowRepository borrowRepo,
        IBookRepository bookRepo,
        ILogger<GetPopularBooksQueryHandler> logger)
    {
        _borrowRepo = borrowRepo;
        _bookRepo = bookRepo;
        _logger = logger;
    }

    public async Task<PagedResult<PopularBookDto>> Handle(GetPopularBooksQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating Popular Books Report via Domain Repositories.");

        var (borrows, totalDistinctBooks) = await _borrowRepo.GetPagedAsync(
            null, null, null, 1, int.MaxValue, cancellationToken,
            request.FromDate, request.ToDate, request.BranchId);

        var groups = borrows
            .GroupBy(b => b.BookId)
            .Select(g => new { BookId = g.Key, BorrowCount = g.Count() })
            .OrderByDescending(x => x.BorrowCount)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var totalCount = groups.Count;

        var bookIds = groups.Select(g => g.BookId).ToList();
        var books = bookIds.Count > 0
            ? await _bookRepo.GetByIdsAsync(bookIds, cancellationToken)
            : new List<Book>();

        var bookMap = books.ToDictionary(b => b.Id);
        var authors = await _bookRepo.GetAllAuthorsAsync(cancellationToken);
        var authorMap = authors.ToDictionary(a => a.Id);
        var categories = await _bookRepo.GetAllCategoriesAsync(cancellationToken);
        var categoryMap = categories.ToDictionary(c => c.Id);

        var items = groups.Select(g =>
        {
            var book = bookMap.GetValueOrDefault(g.BookId);
            var author = book != null ? authorMap.GetValueOrDefault(book.AuthorId) : null;
            var category = book != null ? categoryMap.GetValueOrDefault(book.CategoryId) : null;

            return new PopularBookDto
            {
                BookId = g.BookId,
                Title = book?.Title ?? "Unknown Book",
                AuthorName = author?.Name ?? "Unknown Author",
                CategoryName = category?.Name ?? "Unknown Category",
                TotalBorrows = g.BorrowCount
            };
        }).ToList();

        return PagedResult<PopularBookDto>.Create(items, totalCount, request.Page, request.PageSize);
    }
}
