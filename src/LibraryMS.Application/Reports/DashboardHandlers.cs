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

public sealed class GetDashboardSummaryQueryHandler : IRequestHandler<GetDashboardSummaryQuery, DashboardSummaryDto>
{
    private readonly IBorrowRepository _borrowRepo;
    private readonly IBookRepository _bookRepo;
    private readonly IMemberRepository _memberRepo;
    private readonly IBranchRepository _branchRepo;
    private readonly IReservationRepository _reservationRepo;
    private readonly ILogger<GetDashboardSummaryQueryHandler> _logger;

    public GetDashboardSummaryQueryHandler(
        IBorrowRepository borrowRepo,
        IBookRepository bookRepo,
        IMemberRepository memberRepo,
        IBranchRepository branchRepo,
        IReservationRepository reservationRepo,
        ILogger<GetDashboardSummaryQueryHandler> logger)
    {
        _borrowRepo = borrowRepo;
        _bookRepo = bookRepo;
        _memberRepo = memberRepo;
        _branchRepo = branchRepo;
        _reservationRepo = reservationRepo;
        _logger = logger;
    }

    public async Task<DashboardSummaryDto> Handle(
        GetDashboardSummaryQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving dashboard summary counts.");

        var activeBorrows = await _borrowRepo.GetPagedAsync(null, null, "Active", 1, 1, cancellationToken, null, null, request.BranchId);
        var overdueBorrows = await _borrowRepo.GetPagedAsync(null, null, "Overdue", 1, 1, cancellationToken, null, null, request.BranchId);
        var books = await _bookRepo.SearchAsync(null, null, null, null, 1, 1, cancellationToken);
        var members = await _memberRepo.SearchAsync(null, null, 1, 1, cancellationToken);
        var branches = await _branchRepo.GetAllAsync(cancellationToken);
        var pendingReservations = await _reservationRepo.GetPendingCountAsync(request.BranchId, cancellationToken);
        var totalLateFinesCollected = await _borrowRepo.GetTotalLateFinesCollectedAsync(request.BranchId, cancellationToken);
        var pendingLateFines = await _borrowRepo.GetPendingLateFinesAsync(request.BranchId, cancellationToken);

        _logger.LogInformation("Successfully retrieved dashboard summary.");

        return new DashboardSummaryDto
        {
            TotalBooks = books.TotalCount,
            TotalMembers = members.TotalCount,
            ActiveBorrows = activeBorrows.TotalCount,
            OverdueBorrows = overdueBorrows.TotalCount,
            TotalBranches = branches.Count(b => b.IsActive),
            PendingReservations = pendingReservations,
            TotalLateFinesCollected = totalLateFinesCollected,
            PendingLateFines = pendingLateFines,
        };
    }
}
