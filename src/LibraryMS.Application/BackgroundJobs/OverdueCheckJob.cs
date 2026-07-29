using LibraryMS.Domain.BorrowManagement;
using LibraryMS.Domain.BorrowManagement.AggregateRoots;
using LibraryMS.Domain.BorrowManagement.Services;
using LibraryMS.Domain.Shared;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.BackgroundJobs;

// Background job service — called by Hangfire from Infrastructure layer.
// Contains pure application logic without infrastructure concerns.
public sealed class OverdueCheckJob
{
    private readonly IBorrowRepository _borrowRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<OverdueCheckJob> _logger;

    public OverdueCheckJob(
        IBorrowRepository borrowRepo,
        IUnitOfWork unitOfWork,
        ILogger<OverdueCheckJob> logger)
    {
        _borrowRepo = borrowRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    // Runs daily: marks all Active borrows past due date as Overdue.
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting overdue check background job at {Time}", DateTime.UtcNow);

        var activeBorrows = await _borrowRepo.GetOverdueBorrowsAsync(cancellationToken);
        var processedCount = 0;

        foreach (var borrow in activeBorrows)
        {
            _logger.LogDebug("Marking borrow record {BorrowId} for member {MemberId} as overdue.", borrow.Id, borrow.MemberId);
            borrow.MarkAsOverdue();
            await _borrowRepo.UpdateAsync(borrow, cancellationToken);
            processedCount++;
        }

        if (processedCount > 0)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Overdue check job completed: {Count} borrows successfully marked as overdue.", processedCount);
        }
        else
        {
            _logger.LogInformation("Overdue check job completed: No overdue borrows found.");
        }
    }
}

