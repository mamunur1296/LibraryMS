using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.BookManagement.AggregateRoots;
using LibraryMS.Domain.BookManagement.Entities;
using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.Shared.Exceptions;

namespace LibraryMS.Domain.BorrowManagement;

/// <summary>
/// Domain Service that orchestrates the Borrow and Return workflow.
/// Enforces all cross-aggregate business rules.
/// </summary>
public sealed class BorrowManager
{
    private readonly IBorrowRepository _borrowRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IBookRepository _bookRepository;

    public BorrowManager(
        IBorrowRepository borrowRepository,
        IMemberRepository memberRepository,
        IBookRepository bookRepository)
    {
        _borrowRepository = borrowRepository;
        _memberRepository = memberRepository;
        _bookRepository = bookRepository;
    }

    /// <summary>
    /// Orchestrates the full borrow workflow:
    ///   1. Validates member eligibility
    ///   2. Checks active borrow limit (max 5)
    ///   3. Marks the copy as borrowed
    ///   4. Creates the borrow record
    /// </summary>
    public async Task<BorrowRecord> BorrowAsync(
        Guid memberId, Guid bookCopyId, Guid bookId, Guid branchId,
        int borrowDays = BorrowRecord.MaxBorrowDays,
        CancellationToken ct = default)
    {
        // Rule 1: Member must exist and be active
        var member = await _memberRepository.GetByIdAsync(memberId, ct)
            ?? throw new NotFoundException(nameof(Member), memberId);

        if (!member.CanBorrow())
            throw new DomainException(
                $"Member '{member.FullName}' is suspended and cannot borrow books.",
                "BORROW_MEMBER_SUSPENDED");

        // Rule 2: Max 5 active borrows
        var activeBorrows = await _memberRepository.GetActiveBorrowCountAsync(memberId, ct);
        if (activeBorrows >= BorrowRecord.MaxActiveBorrowsPerMember)
            throw new DomainException(
                $"Member has reached the maximum of {BorrowRecord.MaxActiveBorrowsPerMember} active borrows.",
                "BORROW_MAX_LIMIT_REACHED");

        // Rule 3: Check if copy is already borrowed
        var alreadyBorrowed = await _borrowRepository.HasActiveBorrowForCopyAsync(bookCopyId, ct);
        if (alreadyBorrowed)
            throw new DomainException("This copy is already borrowed by someone else.", "COPY_ALREADY_BORROWED");

        // Rule 4: Mark book copy as borrowed
        var book = await _bookRepository.GetByIdWithCopiesAsync(bookId, ct)
            ?? throw new NotFoundException(nameof(Book), bookId);

        book.BorrowCopy(bookCopyId);
        await _bookRepository.UpdateAsync(book, ct);

        // Rule 5: Create borrow record
        var borrow = new BorrowRecord(Guid.NewGuid(), memberId, bookCopyId, bookId, branchId, borrowDays);
        await _borrowRepository.AddAsync(borrow, ct);

        return borrow;
    }

    /// <summary>
    /// Orchestrates the return workflow:
    ///   1. Validates the borrow record
    ///   2. Returns the copy (marks as available)
    ///   3. Calculates late fine
    ///   4. Triggers reservation queue check
    /// </summary>
    public async Task<BorrowRecord> ReturnAsync(
        Guid borrowId, string? notes = null,
        CancellationToken ct = default)
    {
        var borrow = await _borrowRepository.GetByIdAsync(borrowId, ct)
            ?? throw new NotFoundException(nameof(BorrowRecord), borrowId);

        // Process return (calculates late fine internally)
        borrow.Return(notes);

        // Mark the copy as available again
        var book = await _bookRepository.GetByIdWithCopiesAsync(borrow.BookId, ct)
            ?? throw new NotFoundException(nameof(Book), borrow.BookId);

        book.ReturnCopy(borrow.BookCopyId);
        await _bookRepository.UpdateAsync(book, ct);
        await _borrowRepository.UpdateAsync(borrow, ct);

        return borrow;
    }
}
