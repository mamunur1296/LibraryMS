using LibraryMS.Domain.BorrowManagement;
using LibraryMS.Domain.Shared;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.BackgroundJobs;

public sealed class DailyFineAccumulationJob
{
    private readonly IBorrowRepository _borrowRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DailyFineAccumulationJob> _logger;

    public DailyFineAccumulationJob(
        IBorrowRepository borrowRepository,
        IUnitOfWork unitOfWork,
        ILogger<DailyFineAccumulationJob> logger)
    {
        _borrowRepository = borrowRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting DailyFineAccumulationJob...");

        var overdues = await _borrowRepository.GetOverdueBorrowsAsync(cancellationToken);
        
        foreach (var borrow in overdues)
        {
            borrow.AccumulateFine();
            await _borrowRepository.UpdateAsync(borrow, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Finished DailyFineAccumulationJob. Processed {Count} overdues.", overdues.Count);
    }
}
