using LibraryMS.Domain.BorrowManagement;
using LibraryMS.Domain.ReservationManagement;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.BackgroundJobs;

/// <summary>
/// Background job services — called by Hangfire from Infrastructure layer.
/// Contains pure application logic without infrastructure concerns.
/// </summary>
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

    /// <summary>
    /// Runs daily: marks all Active borrows past due date as Overdue.
    /// Calculates accrued fines.
    /// </summary>
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting overdue check job at {Time}", DateTime.UtcNow);

        var activeBorrows = await _borrowRepo.GetOverdueBorrowsAsync(cancellationToken);
        var processedCount = 0;

        foreach (var borrow in activeBorrows)
        {
            var markOverdueMethod = typeof(BorrowRecord).GetMethod("MarkAsOverdue",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            markOverdueMethod?.Invoke(borrow, null);
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

/// <summary>Expires reservations that the member didn't act on within 3 days.</summary>
public sealed class ReservationExpiryJob
{
    private readonly IReservationRepository _reservationRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ReservationExpiryJob> _logger;

    public ReservationExpiryJob(
        IReservationRepository reservationRepo,
        IUnitOfWork unitOfWork,
        ILogger<ReservationExpiryJob> logger)
    {
        _reservationRepo = reservationRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var expiredReservations = await _reservationRepo.GetExpiredReservationsAsync(cancellationToken);

        foreach (var reservation in expiredReservations)
        {
            if (!reservation.IsExpiredByTime) continue;

            var expireMethod = typeof(Domain.ReservationManagement.Reservation).GetMethod("Expire",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            expireMethod?.Invoke(reservation, null);
            await _reservationRepo.UpdateAsync(reservation, cancellationToken);
        }

        if (expiredReservations.Count > 0)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("{Count} reservations expired", expiredReservations.Count);
        }
    }
}
