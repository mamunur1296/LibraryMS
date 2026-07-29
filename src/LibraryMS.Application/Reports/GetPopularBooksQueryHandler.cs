using LibraryMS.Application.Contracts.Common;
using LibraryMS.Application.Contracts.DTOs.Report;
using LibraryMS.Application.Contracts.Reports;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.BorrowManagement;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

        var (borrows, _) = await _borrowRepo.GetPagedAsync(
            null, null, null, 1, int.MaxValue, cancellationToken);

        var query = borrows.AsQueryable();

        if (request.FromDate.HasValue)
            query = query.Where(b => b.BorrowDate >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            query = query.Where(b => b.BorrowDate <= request.ToDate.Value);

        if (request.BranchId.HasValue)
            query = query.Where(b => b.BranchId == request.BranchId.Value);

        var popularGroups = query
            .GroupBy(b => b.BookId)
            .Select(g => new { BookId = g.Key, BorrowCount = g.Count() })
            .OrderByDescending(x => x.BorrowCount)
            .ToList();

        var totalCount = popularGroups.Count;

        var pagedGroups = popularGroups
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var items = new List<PopularBookDto>();
        var authors = await _bookRepo.GetAllAuthorsAsync(cancellationToken);
        var categories = await _bookRepo.GetAllCategoriesAsync(cancellationToken);

        foreach (var pg in pagedGroups)
        {
            var book = await _bookRepo.GetByIdAsync(pg.BookId, cancellationToken);
            var author = book != null ? authors.FirstOrDefault(a => a.Id == book.AuthorId) : null;
            var category = book != null ? categories.FirstOrDefault(c => c.Id == book.CategoryId) : null;

            items.Add(new PopularBookDto
            {
                BookId = pg.BookId,
                Title = book?.Title ?? "Unknown Book",
                AuthorName = author?.Name ?? "Unknown Author",
                CategoryName = category?.Name ?? "Unknown Category",
                TotalBorrows = pg.BorrowCount
            });
        }

        return PagedResult<PopularBookDto>.Create(items, totalCount, request.Page, request.PageSize);
    }
}
