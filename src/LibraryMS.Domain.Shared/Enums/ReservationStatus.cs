namespace LibraryMS.Domain.Shared.Enums;

/// <summary>Reservation queue item status</summary>
public enum ReservationStatus
{
    Pending = 1,
    Available = 2,   // copy became available, member notified
    Expired = 3,     // member didn't pick up in time
    Cancelled = 4,
    Fulfilled = 5    // member picked up the book
}
