using LibraryMS.Domain.BorrowManagement.Events;
using LibraryMS.Domain.Common;
using LibraryMS.Domain.Shared.Enums;
using LibraryMS.Domain.Shared.Guards;

namespace LibraryMS.Domain.BorrowManagement.AggregateRoots;

// BorrowRecord — Aggregate Root for the borrow-return transaction.
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
    public Guid? IssuedById { get; private set; }
    public Guid? ReturnedById { get; private set; }

    private BorrowRecord() { }

    internal BorrowRecord(Guid id, Guid memberId, Guid bookCopyId, Guid bookId,
        Guid branchId, Guid? issuedById = null, int borrowDays = MaxBorrowDays)
        : base(id)
    {
        MemberId = memberId;
        BookCopyId = bookCopyId;
        BookId = bookId;
        BranchId = branchId;
        IssuedById = issuedById;
        BorrowDate = DateTime.UtcNow;
        DueDate = BorrowDate.AddDays(borrowDays);
        Status = BorrowStatus.Active;
        LateFine = 0;
        CreatedAt = DateTime.UtcNow;

        AddDomainEvent(new BookBorrowedEvent(id, memberId, bookCopyId, bookId, DueDate));
    }

    internal void Return(string? notes = null, Guid? returnedById = null)
    {
        Ensure.Against(Status == BorrowStatus.Returned, "This book has already been returned.", "BORROW_ALREADY_RETURNED");

        ReturnDate = DateTime.UtcNow;
        Status = BorrowStatus.Returned;
        Notes = notes;
        ReturnedById = returnedById;
        LateFine = CalculateLateFine();

        AddDomainEvent(new BookReturnedEvent(Id, MemberId, BookCopyId, BookId, LateFine));
    }

    internal void MarkAsOverdue()
    {
        if (Status == BorrowStatus.Active && DateTime.UtcNow > DueDate)
        {
            Status = BorrowStatus.Overdue;
            LateFine = CalculateLateFine();
            AddDomainEvent(new BorrowOverdueEvent(Id, MemberId, BookId, DueDate));
        }
    }

    internal void AccumulateFine()
    {
        if (Status == BorrowStatus.Overdue)
        {
            LateFine = CalculateLateFine();
        }
    }

    internal void PayFine()
    {
        Ensure.Against(LateFine <= 0, "No fine to pay.", "BORROW_NO_FINE");
        IsFinePaid = true;
    }

    // Calculates the late fine based on days overdue.
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

