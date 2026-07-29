using LibraryMS.Domain.BranchManagement;
using LibraryMS.Domain.BranchManagement.AggregateRoots;
using Microsoft.EntityFrameworkCore;

namespace LibraryMS.EntityFrameworkCore.Repositories;

public sealed class BranchRepository : BaseRepository<Branch>, IBranchRepository
{
    public BranchRepository(LibraryDbContext dbContext) : base(dbContext) { }

    public async Task<Branch?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(b => b.Name.ToLower() == name.ToLower(), cancellationToken);
    }

    public async Task<bool> ExistsWithNameAsync(string name, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = DbSet.Where(b => b.Name.ToLower() == name.ToLower());
        if (excludeId.HasValue)
            query = query.Where(b => b.Id != excludeId.Value);

        return await query.AnyAsync(cancellationToken);
    }
}

