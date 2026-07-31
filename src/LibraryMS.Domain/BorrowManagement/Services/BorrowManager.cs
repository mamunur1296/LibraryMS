using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.BookManagement.AggregateRoots;
using LibraryMS.Domain.BorrowManagement.AggregateRoots;
using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.MemberManagement.AggregateRoots;
using LibraryMS.Domain.MemberManagement.Services;
using LibraryMS.Domain.Shared.Guards;

namespace LibraryMS.Domain.BorrowManagement.Services;

// Domain Service that orchestrates the Borrow and Return workflow.
// Enforces all cross-aggregate business rules.
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

    // Orchestrates the full borrow workflow:
    public async Task<BorrowRecord> BorrowAsync(
        Guid memberId, Guid bookCopyId, Guid bookId, Guid branchId,
        Guid? issuedById = null,
        int borrowDays = BorrowRecord.MaxBorrowDays,
        CancellationToken ct = default)
    {
        // Rule 1: Member must exist and be active
        var member = await _memberRepository.GetByIdAsync(memberId, ct);
        Ensure.Found(member, nameof(Member), memberId);

        Ensure.Against(!member!.CanBorrow(), $"Member '{member.FullName}' is suspended and cannot borrow books.", "BORROW_MEMBER_SUSPENDED");

        // Rule 1.5: Check for unpaid fines
        var hasUnpaidFine = await _borrowRepository.HasUnpaidFineAsync(memberId, ct);
        Ensure.Against(hasUnpaidFine, "Member has unpaid fines and cannot borrow books.", "BORROW_MEMBER_HAS_FINE");

        // Rule 2: Max 5 active borrows
        var activeBorrows = await _memberRepository.GetActiveBorrowCountAsync(memberId, ct);
        Ensure.Against(activeBorrows >= BorrowRecord.MaxActiveBorrowsPerMember, $"Member has reached the maximum of {BorrowRecord.MaxActiveBorrowsPerMember} active borrows.", "BORROW_MAX_LIMIT_REACHED");

        // Rule 3: Check if copy is already borrowed
        var alreadyBorrowed = await _borrowRepository.HasActiveBorrowForCopyAsync(bookCopyId, ct);
        Ensure.Against(alreadyBorrowed, "This copy is already borrowed by someone else.", "COPY_ALREADY_BORROWED");

        // Rule 4: Mark book copy as borrowed
        var book = await _bookRepository.GetByIdWithCopiesAsync(bookId, ct);
        Ensure.Found(book, nameof(Book), bookId);

        book!.BorrowCopy(bookCopyId);
        await _bookRepository.UpdateAsync(book, ct);

        // Rule 5: Create borrow record
        var borrowRecord = new BorrowRecord(
            Guid.NewGuid(), memberId, bookCopyId, bookId, branchId, issuedById, borrowDays);
        await _borrowRepository.AddAsync(borrowRecord, ct);

        return borrowRecord;
    }

    // Orchestrates the return
    public async Task<BorrowRecord> ReturnAsync(
        Guid borrowRecordId, string? notes = null, Guid? returnedById = null, CancellationToken ct = default)
    {
        var record = await _borrowRepository.GetByIdAsync(borrowRecordId, ct);
        Ensure.Found(record, nameof(BorrowRecord), borrowRecordId);

        // Update book copy
        var book = await _bookRepository.GetByIdWithCopiesAsync(record!.BookId, ct);
        Ensure.Found(book, nameof(Book), record.BookId);
        
        book!.ReturnCopy(record.BookCopyId);
        await _bookRepository.UpdateAsync(book, ct);

        // Update borrow record
        record.Return(notes, returnedById);
        await _borrowRepository.UpdateAsync(record, ct);

        return record;
    }
}
