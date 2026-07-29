using LibraryMS.Domain.BranchManagement.AggregateRoots;

namespace LibraryMS.Domain.BranchManagement;

// Repository contract for Branch aggregate — defined in Domain, implemented in Infrastructure.
public interface IBranchRepository
{
    Task<Branch?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<Branch>> GetAllAsync(CancellationToken ct = default);
    Task<bool> ExistsWithNameAsync(string name, Guid? excludeId, CancellationToken ct = default);
    Task AddAsync(Branch branch, CancellationToken ct = default);
    Task UpdateAsync(Branch branch, CancellationToken ct = default);
    Task DeleteAsync(Branch branch, CancellationToken ct = default);
}
