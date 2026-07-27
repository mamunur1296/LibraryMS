using LibraryMS.Domain.Common;
using LibraryMS.Domain.Shared.Enums;
using LibraryMS.Domain.Shared.Exceptions;

namespace LibraryMS.Domain.BookManagement;

/// <summary>
/// Represents a physical copy of a Book located in a specific Branch.
/// Status tracks availability for borrow/reservation.
/// </summary>
public sealed class BookCopy : Entity<Guid>
{
    public Guid BookId { get; private set; }
    public Guid BranchId { get; private set; }
    public string CopyNumber { get; private set; } = default!;  // e.g., "B001-C003"
    public CopyStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation - EF Core
    public Book Book { get; private set; } = default!;

    private BookCopy() { }

    internal BookCopy(Guid id, Guid bookId, Guid branchId, string copyNumber)
        : base(id)
    {
        BookId = bookId;
        BranchId = branchId;
        CopyNumber = copyNumber;
        Status = CopyStatus.Available;
        CreatedAt = DateTime.UtcNow;
    }

    internal void MarkAsBorrowed()
    {
        if (Status != CopyStatus.Available)
            throw new DomainException($"Copy '{CopyNumber}' is not available for borrowing. Current status: {Status}",
                "COPY_NOT_AVAILABLE");
        Status = CopyStatus.Borrowed;
        UpdatedAt = DateTime.UtcNow;
    }

    internal void MarkAsAvailable()
    {
        Status = CopyStatus.Available;
        UpdatedAt = DateTime.UtcNow;
    }

    internal void MarkAsReserved()
    {
        if (Status != CopyStatus.Available)
            throw new DomainException($"Copy '{CopyNumber}' cannot be reserved. Current status: {Status}",
                "COPY_NOT_RESERVABLE");
        Status = CopyStatus.Reserved;
        UpdatedAt = DateTime.UtcNow;
    }

    internal void MarkAsDamaged()
    {
        Status = CopyStatus.Damaged;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsAvailable => Status == CopyStatus.Available;
}
