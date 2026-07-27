namespace LibraryMS.Domain.Shared.Errors;

public static class LibraryMSErrorCodes
{
    public const string BookNotFound = "LibraryMS:Book:0001";
    public const string BookCopyNotFound = "LibraryMS:BookCopy:0001";
    public const string BookCopyNotAvailable = "LibraryMS:BookCopy:0002";
    
    public const string MemberNotFound = "LibraryMS:Member:0001";
    public const string MemberSuspended = "LibraryMS:Member:0002";
    public const string MemberMaxBorrowsReached = "LibraryMS:Member:0003";
    public const string MemberHasOverdueBooks = "LibraryMS:Member:0004";
    
    public const string BranchNotFound = "LibraryMS:Branch:0001";
    
    public const string BorrowNotFound = "LibraryMS:Borrow:0001";
    public const string BorrowAlreadyReturned = "LibraryMS:Borrow:0002";
    
    public const string ReservationNotFound = "LibraryMS:Reservation:0001";
    public const string ReservationAlreadyFulfilled = "LibraryMS:Reservation:0002";
    public const string ReservationAlreadyCancelled = "LibraryMS:Reservation:0003";
}
