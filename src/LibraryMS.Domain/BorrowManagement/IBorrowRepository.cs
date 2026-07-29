using LibraryMS.Domain.BorrowManagement.AggregateRoots;

namespace LibraryMS.Domain.BorrowManagement;

// Repository contract for BorrowRecord aggregate.
public interface IBorrowRepository
{
    Task<BorrowRecord?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<BorrowRecord>> GetActiveBorrowsByMemberAsync(Guid memberId, CancellationToken ct = default);
    Task<List<BorrowRecord>> GetOverdueBorrowsAsync(CancellationToken ct = default);
    Task<bool> HasActiveBorrowForCopyAsync(Guid copyId, CancellationToken ct = default);
    Task<(List<BorrowRecord> Items, int TotalCount)> GetPagedAsync(
        Guid? memberId, Guid? bookId, string? status,
        int page, int pageSize, CancellationToken ct = default);
    Task AddAsync(BorrowRecord record, CancellationToken ct = default);
    Task UpdateAsync(BorrowRecord record, CancellationToken ct = default);
}
