namespace LibraryMS.Application.Contracts.DTOs.Reservation;

public sealed class ReservationQueueDto
{
    public Guid BookId { get; init; }
    public string BookTitle { get; init; } = default!;
    public Guid BranchId { get; init; }
    public string BranchName { get; init; } = default!;
    public int TotalInQueue { get; init; }
    public List<ReservationDto> Queue { get; init; } = [];
}
