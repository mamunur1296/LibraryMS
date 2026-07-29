using LibraryMS.Domain.BranchManagement.Events;
using LibraryMS.Domain.Common;
using LibraryMS.Domain.Shared.Guards;

namespace LibraryMS.Domain.BranchManagement.AggregateRoots;

// Branch — a physical library location that holds book copies.
public sealed class Branch : AggregateRoot<Guid>
{
    public string Name { get; private set; } = default!;
    public string Address { get; private set; } = default!;
    public string Phone { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public bool IsActive { get; private set; } = true;

    private Branch() { } // Required for EF Core

    internal Branch(Guid id, string name, string address, string phone, string email)
        : base(id)
    {
        SetName(name);
        SetAddress(address);
        SetPhone(phone);
        SetEmail(email);
        CreatedAt = DateTime.UtcNow;

        AddDomainEvent(new BranchCreatedEvent(id, name));
    }

    internal void Update(string name, string address, string phone, string email)
    {
        SetName(name);
        SetAddress(address);
        SetPhone(phone);
        SetEmail(email);
        LastModifiedAt = DateTime.UtcNow;
    }

    internal void Deactivate()
    {
        Ensure.Against(!IsActive, "Branch is already inactive.", "BRANCH_ALREADY_INACTIVE");

        IsActive = false;
        LastModifiedAt = DateTime.UtcNow;
    }

    internal void Activate()
    {
        Ensure.Against(IsActive, "Branch is already active.", "BRANCH_ALREADY_ACTIVE");

        IsActive = true;
        LastModifiedAt = DateTime.UtcNow;
    }

    private void SetName(string name)
    {
        Ensure.Against(string.IsNullOrWhiteSpace(name), "Branch name cannot be empty.", "BRANCH_NAME_EMPTY");
        Ensure.Against(name.Length > 200, "Branch name cannot exceed 200 characters.", "BRANCH_NAME_TOO_LONG");
        Name = name.Trim();
    }

    private void SetAddress(string address)
    {
        Ensure.Against(string.IsNullOrWhiteSpace(address), "Branch address cannot be empty.", "BRANCH_ADDRESS_EMPTY");
        Address = address.Trim();
    }

    private void SetPhone(string phone)
    {
        Ensure.Against(string.IsNullOrWhiteSpace(phone), "Branch phone cannot be empty.", "BRANCH_PHONE_EMPTY");
        Phone = phone.Trim();
    }

    private void SetEmail(string email)
    {
        Ensure.Against(string.IsNullOrWhiteSpace(email), "Branch email cannot be empty.", "BRANCH_EMAIL_EMPTY");
        Email = email.Trim().ToLowerInvariant();
    }
}


