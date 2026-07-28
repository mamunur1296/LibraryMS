using LibraryMS.Application.Mapping;
using LibraryMS.Application.Contracts.Reservations;
using LibraryMS.Application.Contracts.DTOs.Reservation;
using LibraryMS.Application.Contracts.Common;
using LibraryMS.Domain.ReservationManagement;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryMS.Application.Reservations;

public sealed class GetReservationsQueryHandler : IRequestHandler<GetReservationsQuery, PagedResult<ReservationDto>>
{
    private readonly IReservationRepository _repository;

    public GetReservationsQueryHandler(IReservationRepository repository)
    {
        _repository = repository; 
    }

    public async Task<PagedResult<ReservationDto>> Handle(GetReservationsQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await _repository.GetPagedAsync(
            request.MemberId, request.BookId, request.Status,
            request.Page, request.PageSize, cancellationToken);

        return PagedResult<ReservationDto>.Create(
            items.Select(i => i.ToDto()).ToList(),
            total, request.Page, request.PageSize);
    }
}
