using LibraryMS.Application.Contracts.Common;
using LibraryMS.Application.Contracts.DTOs.Report;
using LibraryMS.Application.Contracts.Reports;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.BookManagement.AggregateRoots;
using LibraryMS.Domain.BorrowManagement;
using LibraryMS.Domain.BorrowManagement.AggregateRoots;
using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.MemberManagement.AggregateRoots;
using LibraryMS.Domain.BranchManagement;
using LibraryMS.Domain.BranchManagement.AggregateRoots;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Reports;

public sealed class GetOverdueReportQueryHandler : IRequestHandler<GetOverdueReportQuery, PagedResult<OverdueReportDto>>
{
    private readonly IBorrowRepository _borrowRepo;
    private readonly IBookRepository _bookRepo;
    private readonly IMemberRepository _memberRepo;
    private readonly IBranchRepository _branchRepo;
    private readonly ILogger<GetOverdueReportQueryHandler> _logger;

    public GetOverdueReportQueryHandler(
        IBorrowRepository borrowRepo,
        IBookRepository bookRepo,
        IMemberRepository memberRepo,
        IBranchRepository branchRepo,
        ILogger<GetOverdueReportQueryHandler> logger)
    {
        _borrowRepo = borrowRepo;
        _bookRepo = bookRepo;
        _memberRepo = memberRepo;
        _branchRepo = branchRepo;
        _logger = logger;
    }

    public async Task<PagedResult<OverdueReportDto>> Handle(GetOverdueReportQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating Overdue Books Report via Domain Repositories.");

        var (borrows, totalCount) = await _borrowRepo.GetPagedAsync(
            null, null, "Overdue", request.Page, request.PageSize, cancellationToken,
            request.FromDate, request.ToDate, request.BranchId);

        var today = DateTime.UtcNow.Date;
        var memberIds = borrows.Where(b => b.MemberId != Guid.Empty).Select(b => b.MemberId).Distinct().ToList();
        var bookIds = borrows.Where(b => b.BookId != Guid.Empty).Select(b => b.BookId).Distinct().ToList();

        var membersTask = memberIds.Count > 0
            ? _memberRepo.GetByIdsAsync(memberIds, cancellationToken)
            : Task.FromResult(new List<Member>());
        var booksTask = bookIds.Count > 0
            ? _bookRepo.GetByIdsAsync(bookIds, cancellationToken)
            : Task.FromResult(new List<Book>());
        var branchesTask = _branchRepo.GetAllAsync(cancellationToken);

        await Task.WhenAll(membersTask, booksTask, branchesTask);

        var members = membersTask.Result.ToDictionary(m => m.Id);
        var books = booksTask.Result.ToDictionary(b => b.Id);
        var branches = branchesTask.Result.ToDictionary(br => br.Id);

        var items = borrows.Select(b => new OverdueReportDto
        {
            BorrowId = b.Id,
            MemberName = members.TryGetValue(b.MemberId, out var m) ? $"{m.FirstName} {m.LastName}" : "Unknown Member",
            MembershipNumber = m?.MembershipNumber ?? "N/A",
            MemberEmail = m?.Email ?? "N/A",
            BookTitle = books.TryGetValue(b.BookId, out var book) ? book.Title : "Unknown Book",
            BranchName = branches.TryGetValue(b.BranchId, out var br) ? br.Name : "Unknown Branch",
            BorrowDate = b.BorrowDate,
            DueDate = b.DueDate,
            OverdueDays = b.DueDate.Date < today ? (today - b.DueDate.Date).Days : 0,
            AccruedFine = b.LateFine
        }).ToList();

        return PagedResult<OverdueReportDto>.Create(items, totalCount, request.Page, request.PageSize);
    }
}
