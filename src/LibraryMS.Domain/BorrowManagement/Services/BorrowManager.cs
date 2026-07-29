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
        int borrowDays = BorrowRecord.MaxBorrowDays,
        CancellationToken ct = default)
    {
        // Rule 1: Member must exist and be active
        var member = await _memberRepository.GetByIdAsync(memberId, ct);
        Ensure.Found(member, nameof(Member), memberId);

        Ensure.Against(!member!.CanBorrow(), $"Member '{member.FullName}' is suspended and cannot borrow books.", "BORROW_MEMBER_SUSPENDED");

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
        var borrow = new BorrowRecord(Guid.NewGuid(), memberId, bookCopyId, bookId, branchId, borrowDays);
        await _borrowRepository.AddAsync(borrow, ct);

        return borrow;
    }

    // Orchestrates the return
    public async Task<BorrowRecord> ReturnAsync(
        Guid borrowId, string? notes = null,
        CancellationToken ct = default)
    {
        var borrow = await _borrowRepository.GetByIdAsync(borrowId, ct);
        Ensure.Found(borrow, nameof(BorrowRecord), borrowId);

        // Process return (calculates late fine internally)
        borrow!.Return(notes);

        // Mark the copy as available again
        var book = await _bookRepository.GetByIdWithCopiesAsync(borrow.BookId, ct);
        Ensure.Found(book, nameof(Book), borrow.BookId);

        book!.ReturnCopy(borrow.BookCopyId);
        await _bookRepository.UpdateAsync(book, ct);
        await _borrowRepository.UpdateAsync(borrow, ct);

        return borrow;
    }
}

