using LibraryMS.Application.Contracts.Common;
using LibraryMS.Application.Contracts.DTOs.Reservation;
using MediatR;

namespace LibraryMS.Application.Contracts.Reservations;

// ──── Commands ────
public sealed record CreateReservationCommand(
    Guid MemberId, Guid BookId, Guid BranchId)
    : IRequest<ReservationDto>;

public sealed record CancelReservationCommand(Guid Id, Guid RequestingMemberId)
    : IRequest;

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
