using LibraryMS.Application.Contracts.DTOs.Reservation;
using MediatR;

namespace LibraryMS.Application.Contracts.Reservations;

// ──── Commands ────
public sealed record CreateReservationCommand(
    Guid MemberId, Guid BookId, Guid BranchId)
    : IRequest<ReservationDto>;

public sealed record CancelReservationCommand(Guid Id, Guid RequestingMemberId)
    : IRequest;
