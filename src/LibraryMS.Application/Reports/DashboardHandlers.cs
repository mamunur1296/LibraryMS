using LibraryMS.Application.Contracts.DTOs.Report;
using LibraryMS.Application.Contracts.Reports;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.BookManagement.AggregateRoots;
using LibraryMS.Domain.BookManagement.Entities;
using LibraryMS.Domain.BorrowManagement;
using LibraryMS.Domain.BorrowManagement.AggregateRoots;
using LibraryMS.Domain.BorrowManagement.Services;
using LibraryMS.Domain.BranchManagement;
using LibraryMS.Domain.MemberManagement;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Reports;

/// <summary>Provides dashboard summary counts.</summary>
public sealed class GetDashboardSummaryQueryHandler : IRequestHandler<GetDashboardSummaryQuery, DashboardSummaryDto>
{
    private readonly IBorrowRepository _borrowRepo;
    private readonly IBookRepository _bookRepo;
    private readonly IMemberRepository _memberRepo;
    private readonly IBranchRepository _branchRepo;
    private readonly ILogger<GetDashboardSummaryQueryHandler> _logger;

    public GetDashboardSummaryQueryHandler(
        IBorrowRepository borrowRepo,
        IBookRepository bookRepo,
        IMemberRepository memberRepo,
        IBranchRepository branchRepo,
        ILogger<GetDashboardSummaryQueryHandler> logger)
    {
        _borrowRepo = borrowRepo;
        _bookRepo = bookRepo;
        _memberRepo = memberRepo;
        _branchRepo = branchRepo;
        _logger = logger;
    }

    public async Task<DashboardSummaryDto> Handle(
        GetDashboardSummaryQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving dashboard summary counts.");

        var activeBorrows = await _borrowRepo.GetPagedAsync(null, null, "Active", 1, int.MaxValue, cancellationToken);
        var overdueBorrows = await _borrowRepo.GetPagedAsync(null, null, "Overdue", 1, int.MaxValue, cancellationToken);
        var books = await _bookRepo.SearchAsync(null, null, null, null, 1, 1, cancellationToken);
        var members = await _memberRepo.SearchAsync(null, null, 1, 1, cancellationToken);
        var branches = await _branchRepo.GetAllAsync(cancellationToken);

        _logger.LogInformation("Successfully retrieved dashboard summary.");

        return new DashboardSummaryDto
        {
            TotalBooks = books.TotalCount,
            TotalMembers = members.TotalCount,
            ActiveBorrows = activeBorrows.TotalCount,
            OverdueBorrows = overdueBorrows.TotalCount,
            TotalBranches = branches.Count(b => b.IsActive),
        };
    }
}

