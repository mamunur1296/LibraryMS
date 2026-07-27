using LibraryMS.Domain.Common;
using LibraryMS.Domain.Shared.Exceptions;
using LibraryMS.Domain.BranchManagement.Events;

namespace LibraryMS.Domain.BranchManagement;

/// <summary>
/// Branch — a physical library location that holds book copies.
/// </summary>
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
        if (!IsActive)
            throw new DomainException("Branch is already inactive.", "BRANCH_ALREADY_INACTIVE");

        IsActive = false;
        LastModifiedAt = DateTime.UtcNow;
    }

    internal void Activate()
    {
        if (IsActive)
            throw new DomainException("Branch is already active.", "BRANCH_ALREADY_ACTIVE");

        IsActive = true;
        LastModifiedAt = DateTime.UtcNow;
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Branch name cannot be empty.", "BRANCH_NAME_EMPTY");
        if (name.Length > 200)
            throw new DomainException("Branch name cannot exceed 200 characters.", "BRANCH_NAME_TOO_LONG");
        Name = name.Trim();
    }

    private void SetAddress(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            throw new DomainException("Branch address cannot be empty.", "BRANCH_ADDRESS_EMPTY");
        Address = address.Trim();
    }

    private void SetPhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            throw new DomainException("Branch phone cannot be empty.", "BRANCH_PHONE_EMPTY");
        Phone = phone.Trim();
    }

    private void SetEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Branch email cannot be empty.", "BRANCH_EMAIL_EMPTY");
        Email = email.Trim().ToLowerInvariant();
    }
}


