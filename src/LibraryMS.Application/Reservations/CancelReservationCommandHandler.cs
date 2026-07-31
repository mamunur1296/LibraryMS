using LibraryMS.Application.Contracts.Reservations;
using LibraryMS.Domain.ReservationManagement;
using LibraryMS.Domain.Shared;
using LibraryMS.Domain.Shared.Exceptions;
using LibraryMS.Domain.Shared.Guards;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Reservations;

public sealed class CancelReservationCommandHandler : IRequestHandler<CancelReservationCommand>
{
    private readonly IReservationRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CancelReservationCommandHandler> _logger;

    public CancelReservationCommandHandler(
        IReservationRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<CancelReservationCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(CancelReservationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing CancelReservationCommand for ReservationId: {Id}, MemberId: {MemberId}", request.Id, request.RequestingMemberId);

        var reservation = await _repository.GetByIdAsync(request.Id, cancellationToken);
        Ensure.Found(reservation, $"Reservation with ID '{request.Id}' was not found.");

        // Members can only cancel their own reservations
        if (reservation!.MemberId != request.RequestingMemberId)
            throw new ForbiddenException("You can only cancel your own reservations.");

        // Call internal method directly due to InternalsVisibleTo
        reservation.Cancel();

        // Shift queue positions for remaining reservations
        var queue = await _repository.GetQueueForBookAsync(
            reservation.BookId, reservation.BranchId, cancellationToken);

        var subsequent = queue
            .Where(r => r.QueuePosition > reservation.QueuePosition)
            .OrderBy(r => r.QueuePosition)
            .ToList();

        foreach (var r in subsequent)
        {
            // Call internal method directly due to InternalsVisibleTo
            r.UpdateQueuePosition(r.QueuePosition - 1);
        }

        await _repository.UpdateAsync(reservation, cancellationToken);
        if (subsequent.Count > 0)
            await _repository.UpdateRangeAsync(subsequent, cancellationToken);

        var dbFailed = false;
        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cancel reservation with ID {Id} in database.", request.Id);
            dbFailed = true;
        }

        Ensure.Against(dbFailed, "An error occurred while cancelling the reservation in the database.", "DB_UPDATE_ERROR");

        _logger.LogInformation("Reservation {Id} cancelled by member {MemberId}", request.Id, request.RequestingMemberId);
    }
}

