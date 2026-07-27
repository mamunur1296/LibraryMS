namespace LibraryMS.Application.Contracts.DTOs.Reservation;

public sealed class ReservationDto
{
    public Guid Id { get; init; }
    public Guid MemberId { get; init; }
    public string MemberName { get; init; } = default!;
    public string MembershipNumber { get; init; } = default!;
    public Guid BookId { get; init; }
    public string BookTitle { get; init; } = default!;
    public string BookISBN { get; init; } = default!;
    public Guid BranchId { get; init; }
    public string BranchName { get; init; } = default!;
    public int QueuePosition { get; init; }
    public string Status { get; init; } = default!;
    public DateTime CreatedAt { get; init; }
    public DateTime? NotifiedAt { get; init; }
    public DateTime? ExpiresAt { get; init; }
}

public sealed class CreateReservationRequest
{
    public Guid MemberId { get; init; }
    public Guid BookId { get; init; }
    public Guid BranchId { get; init; }
}

public sealed class ReservationQueueDto
{
    public Guid BookId { get; init; }
    public string BookTitle { get; init; } = default!;
    public Guid BranchId { get; init; }
    public string BranchName { get; init; } = default!;
    public int TotalInQueue { get; init; }
    public List<ReservationDto> Queue { get; init; } = [];
}
