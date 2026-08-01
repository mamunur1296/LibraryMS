using LibraryMS.Domain.BorrowManagement.AggregateRoots;

namespace LibraryMS.Domain.BorrowManagement;

// Repository contract for BorrowRecord aggregate.
public interface IBorrowRepository
{
    Task<BorrowRecord?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<BorrowRecord>> GetActiveBorrowsByMemberAsync(Guid memberId, CancellationToken ct = default);
    Task<List<BorrowRecord>> GetOverdueBorrowsAsync(CancellationToken ct = default);
    Task<bool> HasActiveBorrowForCopyAsync(Guid copyId, CancellationToken ct = default);
    Task<bool> HasUnpaidFineAsync(Guid memberId, CancellationToken ct = default);
    Task<(List<BorrowRecord> Items, int TotalCount)> GetPagedAsync(
        Guid? memberId, Guid? bookId, string? status,
        int page, int pageSize, CancellationToken ct = default,
        DateTime? fromDate = null, DateTime? toDate = null, Guid? branchId = null);
    Task<List<BorrowRecord>> GetByMemberIdsAsync(List<Guid> memberIds, DateTime? fromDate, DateTime? toDate, CancellationToken ct = default);
    Task<decimal> GetTotalLateFinesCollectedAsync(Guid? branchId = null, CancellationToken ct = default);
    Task<decimal> GetPendingLateFinesAsync(Guid? branchId = null, CancellationToken ct = default);
    Task AddAsync(BorrowRecord record, CancellationToken ct = default);
    Task UpdateAsync(BorrowRecord record, CancellationToken ct = default);
}
