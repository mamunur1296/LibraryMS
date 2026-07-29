using LibraryMS.Domain.BorrowManagement;
using LibraryMS.Domain.BorrowManagement.AggregateRoots;
using LibraryMS.Domain.BorrowManagement.Services;
using LibraryMS.Domain.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace LibraryMS.EntityFrameworkCore.Repositories;

public sealed class BorrowRepository : BaseRepository<BorrowRecord>, IBorrowRepository
{
    public BorrowRepository(LibraryDbContext dbContext) : base(dbContext) { }

    public async Task<int> GetActiveBorrowCountAsync(Guid memberId, CancellationToken cancellationToken = default)
    {
        return await DbSet.CountAsync(
            r => r.MemberId == memberId && (r.Status == BorrowStatus.Active || r.Status == BorrowStatus.Overdue),
            cancellationToken);
    }

    public async Task<List<BorrowRecord>> GetOverdueBorrowsAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        return await DbSet
            .Where(r => r.Status == BorrowStatus.Active && r.DueDate.Date < today)
            .ToListAsync(cancellationToken);
    }

    public async Task<(List<BorrowRecord> Items, int TotalCount)> GetPagedAsync(
        Guid? memberId, Guid? bookId, string? status, 
        int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking().AsQueryable();

        if (memberId.HasValue)
            query = query.Where(r => r.MemberId == memberId.Value);

        if (bookId.HasValue)
            query = query.Where(r => r.BookId == bookId.Value);

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<BorrowStatus>(status, true, out var parsedStatus))
            query = query.Where(r => r.Status == parsedStatus);

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(r => r.BorrowDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }
    public async Task<List<BorrowRecord>> GetActiveBorrowsByMemberAsync(Guid memberId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(r => r.MemberId == memberId && (r.Status == BorrowStatus.Active || r.Status == BorrowStatus.Overdue))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasActiveBorrowForCopyAsync(Guid bookCopyId, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(
            r => r.BookCopyId == bookCopyId && (r.Status == BorrowStatus.Active || r.Status == BorrowStatus.Overdue),
            cancellationToken);
    }
}

