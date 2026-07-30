using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.MemberManagement.AggregateRoots;
using LibraryMS.Domain.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace LibraryMS.EntityFrameworkCore.Repositories;

public sealed class MemberRepository : BaseRepository<Member>, IMemberRepository
{
    public MemberRepository(LibraryDbContext dbContext) : base(dbContext) { }

    public async Task<List<Member>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        return await DbSet.AsNoTracking().Where(m => ids.Contains(m.Id)).ToListAsync(cancellationToken);
    }

    public async Task<Member?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(m => m.Email.ToLower() == email.ToLower(), cancellationToken);
    }

    public async Task<Member?> GetByMembershipNumberAsync(string membershipNumber, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(m => m.MembershipNumber == membershipNumber, cancellationToken);
    }

    public async Task<(List<Member> Items, int TotalCount)> SearchAsync(
        string? searchTerm, string? status,
        int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            query = query.Where(m =>
                m.FirstName.ToLower().Contains(searchTerm) ||
                m.LastName.ToLower().Contains(searchTerm) ||
                m.Email.ToLower().Contains(searchTerm) ||
                m.MembershipNumber.ToLower().Contains(searchTerm));
        }

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<MemberStatus>(status, true, out var parsedStatus))
            query = query.Where(m => m.Status == parsedStatus);

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(m => m.JoinDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }
    public async Task<bool> EmailExistsAsync(string email, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = DbSet.Where(m => m.Email.ToLower() == email.ToLower());
        if (excludeId.HasValue)
            query = query.Where(m => m.Id != excludeId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> MembershipNumberExistsAsync(string membershipNumber, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(m => m.MembershipNumber == membershipNumber, cancellationToken);
    }

    public async Task<int> GetActiveBorrowCountAsync(Guid memberId, CancellationToken cancellationToken = default)
    {
        return await DbContext.BorrowRecords.CountAsync(r =>
            r.MemberId == memberId &&
            (r.Status == BorrowStatus.Active ||
             r.Status == BorrowStatus.Overdue),
            cancellationToken);
    }
}

