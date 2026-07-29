using LibraryMS.Domain.ReservationManagement;
using LibraryMS.Domain.ReservationManagement.AggregateRoots;
using LibraryMS.Domain.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace LibraryMS.EntityFrameworkCore.Repositories;

public sealed class ReservationRepository : BaseRepository<Reservation>, IReservationRepository
{
    public ReservationRepository(LibraryDbContext dbContext) : base(dbContext) { }

    public async Task<int> GetNextQueuePositionAsync(Guid bookId, Guid branchId, CancellationToken cancellationToken = default)
    {
        var maxPosition = await DbSet
            .Where(r => r.BookId == bookId && r.BranchId == branchId && r.Status == ReservationStatus.Pending)
            .MaxAsync(r => (int?)r.QueuePosition, cancellationToken);
            
        return (maxPosition ?? 0) + 1;
    }

    public async Task<List<Reservation>> GetQueueForBookAsync(Guid bookId, Guid branchId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(r => r.BookId == bookId && r.BranchId == branchId && r.Status == ReservationStatus.Pending)
            .OrderBy(r => r.QueuePosition)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasActiveReservationAsync(Guid memberId, Guid bookId, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(r => 
            r.MemberId == memberId && 
            r.BookId == bookId && 
            (r.Status == ReservationStatus.Pending || r.Status == ReservationStatus.Available),
            cancellationToken);
    }

    public async Task<List<Reservation>> GetExpiredReservationsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(r => r.Status == ReservationStatus.Available && r.ExpiresAt.HasValue && r.ExpiresAt < DateTime.UtcNow)
            .ToListAsync(cancellationToken);
    }

    public async Task<(List<Reservation> Items, int TotalCount)> GetPagedAsync(
        Guid? memberId, Guid? bookId, string? status, 
        int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking().AsQueryable();

        if (memberId.HasValue)
            query = query.Where(r => r.MemberId == memberId.Value);

        if (bookId.HasValue)
            query = query.Where(r => r.BookId == bookId.Value);

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<ReservationStatus>(status, true, out var parsedStatus))
            query = query.Where(r => r.Status == parsedStatus);

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }
}

