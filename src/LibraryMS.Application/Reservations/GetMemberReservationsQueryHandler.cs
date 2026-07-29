using LibraryMS.Application.Contracts.DTOs.Reservation;
using LibraryMS.Application.Contracts.Reservations;
using LibraryMS.Application.Mapping;
using LibraryMS.Domain.ReservationManagement;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryMS.Application.Reservations;

public sealed class GetMemberReservationsQueryHandler : IRequestHandler<GetMemberReservationsQuery, List<ReservationDto>>
{
    private readonly IReservationRepository _reservationRepo;
    private readonly ILogger<GetMemberReservationsQueryHandler> _logger;

    public GetMemberReservationsQueryHandler(
        IReservationRepository reservationRepo,
        ILogger<GetMemberReservationsQueryHandler> logger)
    {
        _reservationRepo = reservationRepo;
        _logger = logger;
    }

    public async Task<List<ReservationDto>> Handle(GetMemberReservationsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving reservations list for Member: {MemberId}", request.MemberId);

        var (reservations, _) = await _reservationRepo.GetPagedAsync(
            request.MemberId, null, null, 1, int.MaxValue, cancellationToken);

        return reservations.Select(r => r.ToDto()).ToList();
    }
}
