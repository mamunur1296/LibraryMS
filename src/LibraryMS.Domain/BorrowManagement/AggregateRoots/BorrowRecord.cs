using LibraryMS.Domain.Common;
using LibraryMS.Domain.BorrowManagement.Events;
using LibraryMS.Domain.Shared.Enums;
using LibraryMS.Domain.Shared.Exceptions;

namespace LibraryMS.Domain.BorrowManagement;

/// <summary>
/// BorrowRecord — Aggregate Root for the borrow-return transaction.
/// Core business rules:
///   - Max 5 active borrows per member
///   - Default 14-day borrow duration
///   - Late fine: ৳2 per day overdue
///   - Suspended members cannot borrow
/// </summary>
public sealed class BorrowRecord : AggregateRoot<Guid>
{
    public const int MaxBorrowDays = 14;
    public const decimal LateFinePerDay = 2.0m;
    public const int MaxActiveBorrowsPerMember = 5;

    public Guid MemberId { get; private set; }
    public Guid BookCopyId { get; private set; }
    public Guid BookId { get; private set; }
    public Guid BranchId { get; private set; }
    public DateTime BorrowDate { get; private set; }
    public DateTime DueDate { get; private set; }
    public DateTime? ReturnDate { get; private set; }
    public BorrowStatus Status { get; private set; }
    public decimal LateFine { get; private set; }
    public bool IsFinePaid { get; private set; }
    public string? Notes { get; private set; }

    private BorrowRecord() { }

    /// <summary>
    /// Creates a new borrow transaction.
    /// Constructor is internal — only BorrowManager can create instances.
    /// </summary>
    internal BorrowRecord(Guid id, Guid memberId, Guid bookCopyId, Guid bookId,
        Guid branchId, int borrowDays = MaxBorrowDays)
        : base(id)
    {
        MemberId = memberId;
        BookCopyId = bookCopyId;
        BookId = bookId;
        BranchId = branchId;
        BorrowDate = DateTime.UtcNow;
        DueDate = BorrowDate.AddDays(borrowDays);
        Status = BorrowStatus.Active;
        LateFine = 0;
        CreatedAt = DateTime.UtcNow;

        AddDomainEvent(new BookBorrowedEvent(id, memberId, bookCopyId, bookId, DueDate));
    }

    /// <summary>
    /// Processes the return of a borrowed book.
    /// Calculates late fine automatically.
    /// </summary>
    internal void Return(string? notes = null)
    {
        if (Status == BorrowStatus.Returned)
            throw new DomainException("This book has already been returned.", "BORROW_ALREADY_RETURNED");

        ReturnDate = DateTime.UtcNow;
        Status = BorrowStatus.Returned;
        Notes = notes;
        LateFine = CalculateLateFine();

        AddDomainEvent(new BookReturnedEvent(Id, MemberId, BookCopyId, BookId, LateFine));
    }

    /// <summary>Marks the borrow as overdue. Called by background job.</summary>
    internal void MarkAsOverdue()
    {
        if (Status == BorrowStatus.Active && DateTime.UtcNow > DueDate)
        {
            Status = BorrowStatus.Overdue;
            LateFine = CalculateLateFine();
            AddDomainEvent(new BorrowOverdueEvent(Id, MemberId, BookId, DueDate));
        }
    }

    internal void PayFine()
    {
        if (LateFine <= 0)
            throw new DomainException("No fine to pay.", "BORROW_NO_FINE");
        IsFinePaid = true;
    }

    /// <summary>Calculates the late fine based on days overdue.</summary>
    private decimal CalculateLateFine()
    {
        var referenceDate = ReturnDate ?? DateTime.UtcNow;
        if (referenceDate <= DueDate) return 0m;

        var overdueDays = (int)(referenceDate - DueDate).TotalDays;
        return overdueDays * LateFinePerDay;
    }

    public bool IsOverdue => Status == BorrowStatus.Active && DateTime.UtcNow > DueDate;
    public int DaysUntilDue => (int)(DueDate - DateTime.UtcNow).TotalDays;
}

