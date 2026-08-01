using LibraryMS.Application.Contracts.DTOs.Report;
using LibraryMS.Application.Contracts.Reports;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.BorrowManagement;
using LibraryMS.Domain.BranchManagement;
using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.ReservationManagement;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Reports;

public sealed class GetAdminDashboardSummaryQueryHandler : IRequestHandler<GetAdminDashboardSummaryQuery, AdminDashboardSummaryDto>
{
    private readonly IBorrowRepository _borrowRepo;
    private readonly IBookRepository _bookRepo;
    private readonly IMemberRepository _memberRepo;
    private readonly IBranchRepository _branchRepo;
    private readonly IReservationRepository _reservationRepo;
    private readonly ILogger<GetAdminDashboardSummaryQueryHandler> _logger;

    public GetAdminDashboardSummaryQueryHandler(
        IBorrowRepository borrowRepo,
        IBookRepository bookRepo,
        IMemberRepository memberRepo,
        IBranchRepository branchRepo,
        IReservationRepository reservationRepo,
        ILogger<GetAdminDashboardSummaryQueryHandler> logger)
    {
        _borrowRepo = borrowRepo;
        _bookRepo = bookRepo;
        _memberRepo = memberRepo;
        _branchRepo = branchRepo;
        _reservationRepo = reservationRepo;
        _logger = logger;
    }

    public async Task<AdminDashboardSummaryDto> Handle(GetAdminDashboardSummaryQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving admin dashboard summary.");

        var branches = await _branchRepo.GetAllAsync(cancellationToken);
        
        // System-wide summary stats
        var globalActiveBorrows = await _borrowRepo.GetPagedAsync(null, null, "Active", 1, 1, cancellationToken);
        var globalOverdueBorrows = await _borrowRepo.GetPagedAsync(null, null, "Overdue", 1, 1, cancellationToken);
        var globalBooks = await _bookRepo.SearchAsync(null, null, null, null, 1, 1, cancellationToken);
        var globalMembers = await _memberRepo.SearchAsync(null, null, 1, 1, cancellationToken);
        var pendingReservations = await _reservationRepo.GetPendingCountAsync(null, cancellationToken);
        var totalLateFinesCollected = await _borrowRepo.GetTotalLateFinesCollectedAsync(null, cancellationToken);
        var pendingLateFines = await _borrowRepo.GetPendingLateFinesAsync(null, cancellationToken);

        var totalSummary = new DashboardSummaryDto
        {
            TotalBooks = globalBooks.TotalCount,
            TotalMembers = globalMembers.TotalCount,
            ActiveBorrows = globalActiveBorrows.TotalCount,
            OverdueBorrows = globalOverdueBorrows.TotalCount,
            TotalBranches = branches.Count(b => b.IsActive),
            PendingReservations = pendingReservations,
            TotalLateFinesCollected = totalLateFinesCollected,
            PendingLateFines = pendingLateFines,
        };
        
        var branchSummaries = new List<BranchDashboardSummaryDto>();
        
        foreach(var branch in branches.Where(b => b.IsActive))
        {
            var branchBorrows = await _borrowRepo.GetPagedAsync(null, branch.Id, null, 1, 100000, cancellationToken);
            var activeBorrows = branchBorrows.Items.Count(b => b.Status == Domain.Shared.Enums.BorrowStatus.Active);
            var overdueBorrows = branchBorrows.Items.Count(b => b.Status == Domain.Shared.Enums.BorrowStatus.Overdue);
            
            var branchBooks = await _bookRepo.SearchAsync(null, null, null, branch.Id, 1, 1, cancellationToken);
            
            branchSummaries.Add(new BranchDashboardSummaryDto
            {
                BranchId = branch.Id,
                BranchName = branch.Name,
                TotalBooks = branchBooks.TotalCount,
                TotalMembers = 0, // Members are global, so branch-level member count is not applicable
                ActiveBorrows = activeBorrows,
                OverdueBorrows = overdueBorrows,
                TotalRevenue = branchBorrows.Items.Where(b => b.IsFinePaid).Sum(b => b.LateFine)
            });
        }
        
        return new AdminDashboardSummaryDto
        {
            TotalSummary = totalSummary,
            BranchSummaries = branchSummaries
        };
    }
}
