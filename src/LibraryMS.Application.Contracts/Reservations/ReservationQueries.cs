using LibraryMS.Application.Contracts.Common;
using LibraryMS.Application.Contracts.DTOs.Reservation;
using MediatR;

namespace LibraryMS.Application.Contracts.Reservations;

// ──── Queries ────
public sealed record GetReservationByIdQuery(Guid Id)
    : IRequest<ReservationDto?>;

public sealed record GetReservationsQuery(
    Guid? MemberId, Guid? BookId, string? Status, int Page, int PageSize)
    : IRequest<PagedResult<ReservationDto>>;

public sealed record GetBookQueueQuery(Guid BookId, Guid BranchId)
    : IRequest<ReservationQueueDto>;

public sealed record GetMemberReservationsQuery(Guid MemberId)
    : IRequest<List<ReservationDto>>;
