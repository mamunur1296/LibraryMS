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
        var expiredReservations = await _reservationRepo.GetExpiredReservationsAsync(cancellationToken);

        foreach (var reservation in expiredReservations)
        {
            if (!reservation.IsExpiredByTime) continue;

            // Call internal method directly due to InternalsVisibleTo
            reservation.Expire();
            await _reservationRepo.UpdateAsync(reservation, cancellationToken);
        }

        if (expiredReservations.Count > 0)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("{Count} reservations expired", expiredReservations.Count);
        }
    }
}
