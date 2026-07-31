using LibraryMS.Domain.MemberManagement.AggregateRoots;

namespace LibraryMS.Domain.MemberManagement;

// Repository contract for Member aggregate.
public interface IMemberRepository
{
    Task<Member?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<Member>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default);
    Task<Member?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<bool> EmailExistsAsync(string email, Guid? excludeId, CancellationToken ct = default);
    Task<bool> MembershipNumberExistsAsync(string number, CancellationToken ct = default);
    Task<(List<Member> Items, int TotalCount)> SearchAsync(
        string? searchTerm, string? status, int page, int pageSize, CancellationToken ct = default);
    Task<int> GetActiveBorrowCountAsync(Guid memberId, CancellationToken ct = default);
    Task AddAsync(Member member, CancellationToken ct = default);
    Task UpdateAsync(Member member, CancellationToken ct = default);
    Task DeleteAsync(Member member, CancellationToken ct = default);
}
