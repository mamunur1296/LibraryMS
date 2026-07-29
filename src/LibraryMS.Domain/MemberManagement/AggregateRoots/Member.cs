using LibraryMS.Domain.Common;
using LibraryMS.Domain.MemberManagement.Events;
using LibraryMS.Domain.Shared.Enums;
using LibraryMS.Domain.Shared.Exceptions;

namespace LibraryMS.Domain.MemberManagement;

/// <summary>
/// Member — a registered library member who can borrow and reserve books.
/// </summary>
public sealed class Member : AggregateRoot<Guid>
{
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string Phone { get; private set; } = default!;
    public string MembershipNumber { get; private set; } = default!;  // e.g., "LIB-2024-00001"
    public string? Address { get; private set; }
    public MemberStatus Status { get; private set; }
    public DateTime JoinDate { get; private set; }
    public DateTime? SuspendedUntil { get; private set; }

    // Optimistic concurrency token
    public byte[] RowVersion { get; private set; } = default!;

    public string FullName => $"{FirstName} {LastName}";

    private Member() { }

    internal Member(Guid id, string firstName, string lastName,
        string email, string phone, string membershipNumber, string? address)
        : base(id)
    {
        SetFirstName(firstName);
        SetLastName(lastName);
        SetEmail(email);
        SetPhone(phone);
        MembershipNumber = membershipNumber;
        Address = address;
        Status = MemberStatus.Active;
        JoinDate = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;

        AddDomainEvent(new MemberRegisteredEvent(id, firstName, lastName, email));
    }

    internal void Update(string firstName, string lastName, string phone, string? address)
    {
        SetFirstName(firstName);
        SetLastName(lastName);
        SetPhone(phone);
        Address = address;
        LastModifiedAt = DateTime.UtcNow;
    }

    internal void Suspend(DateTime until, string reason)
    {
        if (Status == MemberStatus.Suspended)
            throw new DomainException("Member is already suspended.", "MEMBER_ALREADY_SUSPENDED");

        Status = MemberStatus.Suspended;
        SuspendedUntil = until;
        LastModifiedAt = DateTime.UtcNow;

        AddDomainEvent(new MemberSuspendedEvent(Id, FullName, until, reason));
    }

    internal void Activate()
    {
        if (Status == MemberStatus.Active)
            throw new DomainException("Member is already active.", "MEMBER_ALREADY_ACTIVE");

        Status = MemberStatus.Active;
        SuspendedUntil = null;
        LastModifiedAt = DateTime.UtcNow;
    }

    /// <summary>Checks if the member can borrow a book (not suspended, not overdue-blocked).</summary>
    public bool CanBorrow()
    {
        if (Status == MemberStatus.Suspended)
        {
            if (SuspendedUntil.HasValue && SuspendedUntil.Value <= DateTime.UtcNow)
            {
                // Auto-lift expired suspension
                Status = MemberStatus.Active;
                SuspendedUntil = null;
                return true;
            }
            return false;
        }
        return Status == MemberStatus.Active;
    }

    private void SetFirstName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("First name cannot be empty.", "MEMBER_FIRSTNAME_EMPTY");
        FirstName = name.Trim();
    }

    private void SetLastName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Last name cannot be empty.", "MEMBER_LASTNAME_EMPTY");
        LastName = name.Trim();
    }

    private void SetEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email cannot be empty.", "MEMBER_EMAIL_EMPTY");
        Email = email.Trim().ToLowerInvariant();
    }

    private void SetPhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            throw new DomainException("Phone cannot be empty.", "MEMBER_PHONE_EMPTY");
        Phone = phone.Trim();
    }
}


