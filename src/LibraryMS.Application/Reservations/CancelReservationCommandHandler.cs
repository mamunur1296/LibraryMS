using LibraryMS.Domain.Shared;
using LibraryMS.Application.Contracts.Reservations;
using LibraryMS.Domain.ReservationManagement;
using LibraryMS.Domain.Shared.Exceptions;
using LibraryMS.Domain.Shared.Guards;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Reservation {Id} cancelled by member {MemberId}", request.Id, request.RequestingMemberId);
    }
}
