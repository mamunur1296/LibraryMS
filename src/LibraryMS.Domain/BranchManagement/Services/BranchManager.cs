using LibraryMS.Domain.BranchManagement.AggregateRoots;
using LibraryMS.Domain.Shared.Guards;

namespace LibraryMS.Domain.BranchManagement.Services;

// Domain service for creating and managing Branch aggregates.
// Enforces uniqueness and business rules that require repository access.
public sealed class BranchManager
{
    private readonly IBranchRepository _repository;

    public BranchManager(IBranchRepository repository)
        => _repository = repository;

    public async Task<Branch> CreateAsync(
        string name, string address, string phone, string email,
        CancellationToken ct = default)
    {
        await EnsureNameUniqueAsync(name, excludeId: null, ct);

        return new Branch(Guid.NewGuid(), name, address, phone, email);
    }

    public async Task<Branch> UpdateAsync(
        Branch branch, string name, string address, string phone, string email,
        CancellationToken ct = default)
    {
        await EnsureNameUniqueAsync(name, excludeId: branch.Id, ct);

        branch.Update(name, address, phone, email);
        return branch;
    }

    private async Task EnsureNameUniqueAsync(string name, Guid? excludeId, CancellationToken ct)
    {
        var exists = await _repository.ExistsWithNameAsync(name, excludeId, ct);
        Ensure.Against(exists, $"A branch named '{name}' already exists.", "BRANCH_DUPLICATE_NAME");
    }
}
