using LibraryMS.Application.Contracts.Common;
using LibraryMS.Application.Contracts.DTOs.Report;
using LibraryMS.Application.Contracts.Reports;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.BorrowManagement;
using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.BranchManagement;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

        var (borrows, _) = await _borrowRepo.GetPagedAsync(
            null, null, "Overdue", 1, int.MaxValue, cancellationToken);

        var today = DateTime.UtcNow.Date;
        var query = borrows.AsQueryable();

        if (request.FromDate.HasValue)
            query = query.Where(b => b.BorrowDate >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            query = query.Where(b => b.BorrowDate <= request.ToDate.Value);

        if (request.BranchId.HasValue)
            query = query.Where(b => b.BranchId == request.BranchId.Value);

        var filteredBorrows = query.ToList();
        var totalCount = filteredBorrows.Count;

        var pagedBorrows = filteredBorrows
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var items = new List<OverdueReportDto>();
        var branches = await _branchRepo.GetAllAsync(cancellationToken);

        foreach (var b in pagedBorrows)
        {
            var member = await _memberRepo.GetByIdAsync(b.MemberId, cancellationToken);
            var book = await _bookRepo.GetByIdAsync(b.BookId, cancellationToken);
            var branch = branches.FirstOrDefault(br => br.Id == b.BranchId);

            items.Add(new OverdueReportDto
            {
                BorrowId = b.Id,
                MemberName = member != null ? $"{member.FirstName} {member.LastName}" : "Unknown Member",
                MembershipNumber = member?.MembershipNumber ?? "N/A",
                MemberEmail = member?.Email ?? "N/A",
                BookTitle = book?.Title ?? "Unknown Book",
                BranchName = branch?.Name ?? "Unknown Branch",
                BorrowDate = b.BorrowDate,
                DueDate = b.DueDate,
                OverdueDays = b.DueDate.Date < today ? (today - b.DueDate.Date).Days : 0,
                AccruedFine = b.LateFine
            });
        }

        return PagedResult<OverdueReportDto>.Create(items, totalCount, request.Page, request.PageSize);
    }
}
