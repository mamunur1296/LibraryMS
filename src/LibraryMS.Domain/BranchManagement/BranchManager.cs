using LibraryMS.Domain.Common;
using LibraryMS.Domain.Shared.Exceptions;

namespace LibraryMS.Domain.BranchManagement;

/// <summary>
/// Domain service for creating and managing Branch aggregates.
/// Enforces uniqueness and business rules that require repository access.
/// </summary>
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
        if (exists)
            throw new DomainException($"A branch named '{name}' already exists.", "BRANCH_DUPLICATE_NAME");
    }
}
