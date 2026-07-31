using LibraryMS.Domain.ReservationManagement;
using LibraryMS.Domain.Shared;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.BackgroundJobs;

/// Background job service — called by Hangfire from Infrastructure layer.
/// Expires reservations that the member didn't act on within 3 days.
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
        _logger.LogInformation("Starting reservation expiry background job at {Time}", DateTime.UtcNow);

        var expiredReservations = await _reservationRepo.GetExpiredReservationsAsync(cancellationToken);
        var processedCount = 0;

        foreach (var reservation in expiredReservations)
        {
            if (!reservation.IsExpiredByTime) continue;

            _logger.LogDebug("Expiring reservation {ReservationId} for Book {BookId} and Member {MemberId}.", reservation.Id, reservation.BookId, reservation.MemberId);
            reservation.Expire();
            await _reservationRepo.UpdateAsync(reservation, cancellationToken);
            processedCount++;
        }

        if (processedCount > 0)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Reservation expiry job completed: {Count} reservations expired.", processedCount);
        }
        else
        {
            _logger.LogInformation("Reservation expiry job completed: No expired reservations found.");
        }
    }
}

