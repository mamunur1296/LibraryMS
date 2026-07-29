using LibraryMS.Domain.Shared.Exceptions;
using LibraryMS.Domain.Shared.Enums;

namespace LibraryMS.Domain.MemberManagement;

/// <summary>Domain service for creating and managing Member aggregates.</summary>
public sealed class MemberManager
{
    private readonly IMemberRepository _repository;
    private static int _sequenceCounter = 1;

    public MemberManager(IMemberRepository repository)
        => _repository = repository;

    public async Task<Member> CreateAsync(
        string firstName, string lastName, string email, string phone, string? address,
        CancellationToken ct = default)
    {
        await EnsureEmailUniqueAsync(email, excludeId: null, ct);

        var membershipNumber = await GenerateMembershipNumberAsync(ct);

        return new Member(Guid.NewGuid(), firstName, lastName, email, phone, membershipNumber, address);
    }

    public async Task<Member> UpdateAsync(
        Member member, string firstName, string lastName, string phone, string? address,
        CancellationToken ct = default)
    {
        member.Update(firstName, lastName, phone, address);
        return member;
    }

    public void SuspendMember(Member member, DateTime until, string reason)
    {
        ArgumentNullException.ThrowIfNull(member);
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Suspension reason is required.", "MEMBER_SUSPENSION_NO_REASON");

        member.Suspend(until, reason);
    }

    public void ActivateMember(Member member)
    {
        ArgumentNullException.ThrowIfNull(member);
        member.Activate();
    }

    private async Task EnsureEmailUniqueAsync(string email, Guid? excludeId, CancellationToken ct)
    {
        var exists = await _repository.EmailExistsAsync(email, excludeId, ct);
        if (exists)
            throw new DomainException($"A member with email '{email}' already exists.", "MEMBER_DUPLICATE_EMAIL");
    }

    private async Task<string> GenerateMembershipNumberAsync(CancellationToken ct)
    {
        // Format: LIB-YYYY-NNNNN
        var year = DateTime.UtcNow.Year;
        var baseNumber = $"LIB-{year}-";

        // Try up to 100 times to find a unique number
        for (int i = 0; i < 100; i++)
        {
            var number = $"{baseNumber}{_sequenceCounter:D5}";
            _sequenceCounter++;

            if (!await _repository.MembershipNumberExistsAsync(number, ct))
                return number;
        }

        throw new DomainException("Failed to generate a unique membership number.", "MEMBER_NUMBER_GENERATION_FAILED");
    }
}
