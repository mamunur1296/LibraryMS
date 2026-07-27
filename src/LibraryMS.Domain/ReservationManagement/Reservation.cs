using LibraryMS.Domain.Common;
using LibraryMS.Domain.ReservationManagement.Events;
using LibraryMS.Domain.Shared.Enums;
using LibraryMS.Domain.Shared.Exceptions;

namespace LibraryMS.Domain.ReservationManagement;

/// <summary>
/// Reservation — a queue entry for a book at a specific branch.
/// Business Rules:
///   - Queue position maintained (FIFO)
///   - Cannot reserve if a copy is available
///   - Expires 3 days after notification
/// </summary>
public sealed class Reservation : AggregateRoot<Guid>
{
    public const int ExpiryDaysAfterNotification = 3;

    public Guid MemberId { get; private set; }
    public Guid BookId { get; private set; }
    public Guid BranchId { get; private set; }
    public int QueuePosition { get; private set; }
    public ReservationStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? NotifiedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public DateTime? FulfilledAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }

    private Reservation() { }

    internal Reservation(Guid id, Guid memberId, Guid bookId, Guid branchId, int queuePosition)
        : base(id)
    {
        MemberId = memberId;
        BookId = bookId;
        BranchId = branchId;
        QueuePosition = queuePosition;
        Status = ReservationStatus.Pending;
        CreatedAt = DateTime.UtcNow;

        AddDomainEvent(new ReservationCreatedEvent(id, memberId, bookId, queuePosition));
    }

    /// <summary>Called when a copy becomes available for this reservation.</summary>
    internal void NotifyAvailable()
    {
        if (Status != ReservationStatus.Pending)
            throw new DomainException("Only pending reservations can be notified.", "RESERVATION_INVALID_STATE");

        Status = ReservationStatus.Available;
        NotifiedAt = DateTime.UtcNow;
        ExpiresAt = DateTime.UtcNow.AddDays(ExpiryDaysAfterNotification);

        AddDomainEvent(new ReservationAvailableEvent(Id, MemberId, BookId, ExpiresAt.Value));
    }

    /// <summary>Member picked up the book — reservation fulfilled.</summary>
    internal void Fulfill()
    {
        if (Status != ReservationStatus.Available)
            throw new DomainException("Only available reservations can be fulfilled.", "RESERVATION_INVALID_STATE");

        Status = ReservationStatus.Fulfilled;
        FulfilledAt = DateTime.UtcNow;
    }

    /// <summary>Member cancelled the reservation.</summary>
    internal void Cancel()
    {
        if (Status == ReservationStatus.Fulfilled || Status == ReservationStatus.Cancelled)
            throw new DomainException("Reservation cannot be cancelled in its current state.", "RESERVATION_INVALID_STATE");

        Status = ReservationStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
        AddDomainEvent(new ReservationCancelledEvent(Id, MemberId, BookId));
    }

    /// <summary>Reservation expired — member didn't pick up in time.</summary>
    internal void Expire()
    {
        if (Status != ReservationStatus.Available)
            throw new DomainException("Only available reservations can expire.", "RESERVATION_INVALID_STATE");

        Status = ReservationStatus.Expired;
        AddDomainEvent(new ReservationExpiredEvent(Id, MemberId, BookId));
    }

    internal void UpdateQueuePosition(int newPosition)
    {
        if (newPosition < 1)
            throw new DomainException("Queue position must be at least 1.", "RESERVATION_INVALID_POSITION");
        QueuePosition = newPosition;
    }

    public bool IsExpiredByTime => Status == ReservationStatus.Available
        && ExpiresAt.HasValue && DateTime.UtcNow > ExpiresAt.Value;
}
