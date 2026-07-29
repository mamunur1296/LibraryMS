namespace LibraryMS.Application.Contracts.DTOs.Reservation;

public sealed class CreateReservationRequest
{
    public Guid MemberId { get; init; }
    public Guid BookId { get; init; }
    public Guid BranchId { get; init; }
}
