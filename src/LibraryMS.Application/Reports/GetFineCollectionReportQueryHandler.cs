using LibraryMS.Application.Contracts.DTOs.Report;
using LibraryMS.Application.Contracts.Reports;
using LibraryMS.Domain.BorrowManagement;
using LibraryMS.Domain.BranchManagement;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Reports;

public sealed class GetFineCollectionReportQueryHandler : IRequestHandler<GetFineCollectionReportQuery, List<FineCollectionDto>>
{
    private readonly IBorrowRepository _borrowRepository;
    private readonly IBranchRepository _branchRepository;
    private readonly ILogger<GetFineCollectionReportQueryHandler> _logger;

    public GetFineCollectionReportQueryHandler(
        IBorrowRepository borrowRepository,
        IBranchRepository branchRepository,
        ILogger<GetFineCollectionReportQueryHandler> logger)
    {
        _borrowRepository = borrowRepository;
        _branchRepository = branchRepository;
        _logger = logger;
    }

    public async Task<List<FineCollectionDto>> Handle(GetFineCollectionReportQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving fine collection report");

        // 1. Get Branches
        var branches = await _branchRepository.GetAllAsync(cancellationToken);
        if (request.BranchId.HasValue)
        {
            branches = branches.Where(b => b.Id == request.BranchId.Value).ToList();
        }

        var results = new List<FineCollectionDto>();

        foreach (var branch in branches)
        {
            // For a robust implementation, the repository should have a query that directly aggregates this.
            // For now, we simulate this by loading the branch borrows and filtering in-memory for the reporting period.
            var branchBorrows = await _borrowRepository.GetPagedAsync(null, branch.Id, null, 1, 100000, cancellationToken);
            
            var borrowsInPeriod = branchBorrows.Items.Where(b => b.LateFine > 0).AsEnumerable();

            if (request.FromDate.HasValue)
            {
                borrowsInPeriod = borrowsInPeriod.Where(b => b.ReturnDate >= request.FromDate.Value || (b.ReturnDate == null && b.DueDate >= request.FromDate.Value));
            }
            if (request.ToDate.HasValue)
            {
                borrowsInPeriod = borrowsInPeriod.Where(b => b.ReturnDate <= request.ToDate.Value || (b.ReturnDate == null && b.DueDate <= request.ToDate.Value));
            }

            var borrowsList = borrowsInPeriod.ToList();

            var paidBorrows = borrowsList.Where(b => b.IsFinePaid).ToList();
            var pendingBorrows = borrowsList.Where(b => !b.IsFinePaid).ToList();

            results.Add(new FineCollectionDto
            {
                BranchId = branch.Id,
                BranchName = branch.Name,
                TotalFinesCollected = paidBorrows.Sum(b => b.LateFine),
                TotalPendingFines = pendingBorrows.Sum(b => b.LateFine),
                NumberOfFinesPaid = paidBorrows.Count,
                NumberOfPendingFines = pendingBorrows.Count
            });
        }

        return results.OrderByDescending(r => r.TotalFinesCollected).ToList();
    }
}
