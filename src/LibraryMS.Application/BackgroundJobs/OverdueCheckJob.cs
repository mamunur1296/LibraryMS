using LibraryMS.Domain.BorrowManagement;
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
        _logger.LogInformation("Starting overdue check job at {Time}", DateTime.UtcNow);

        var activeBorrows = await _borrowRepo.GetOverdueBorrowsAsync(cancellationToken);
        var processedCount = 0;

        foreach (var borrow in activeBorrows)
        {
            // Call internal method directly due to InternalsVisibleTo
            borrow.MarkAsOverdue();
            await _borrowRepo.UpdateAsync(borrow, cancellationToken);
            processedCount++;
        }

        if (processedCount > 0)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Overdue check: {Count} borrows marked as overdue", processedCount);
        }
        else
        {
            _logger.LogDebug("Overdue check: no new overdue borrows found");
        }
    }
}
