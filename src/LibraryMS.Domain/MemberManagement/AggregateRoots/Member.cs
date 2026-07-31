using LibraryMS.Domain.Common;
using LibraryMS.Domain.MemberManagement.Events;
using LibraryMS.Domain.MemberManagement.Entities;
using LibraryMS.Domain.Shared.Enums;
using LibraryMS.Domain.Shared.Guards;
using LibraryMS.Domain.Shared.Constants;

namespace LibraryMS.Domain.MemberManagement.AggregateRoots;

// Member — a registered library member who can borrow and reserve books.
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
    public DateTime MembershipExpiry { get; private set; }

    // Optimistic concurrency token
    public byte[] RowVersion { get; private set; } = default!;

    private readonly List<MemberFavorite> _favorites = new();
    public IReadOnlyCollection<MemberFavorite> Favorites => _favorites.AsReadOnly();

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
        MembershipExpiry = DateTime.UtcNow.AddYears(1); // Default 1 year
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
        Ensure.Against(Status == MemberStatus.Suspended, "Member is already suspended.", "MEMBER_ALREADY_SUSPENDED");

        Status = MemberStatus.Suspended;
        SuspendedUntil = until;
        LastModifiedAt = DateTime.UtcNow;

        AddDomainEvent(new MemberSuspendedEvent(Id, FullName, until, reason));
    }

    internal void Activate()
    {
        Ensure.Against(Status == MemberStatus.Active, "Member is already active.", "MEMBER_ALREADY_ACTIVE");

        Status = MemberStatus.Active;
        SuspendedUntil = null;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void RenewMembership(int days)
    {
        Ensure.Against(days <= 0, "Renewal days must be greater than zero.", "MEMBER_RENEWAL_INVALID_DAYS");
        
        if (MembershipExpiry < DateTime.UtcNow)
        {
            MembershipExpiry = DateTime.UtcNow.AddDays(days);
        }
        else
        {
            MembershipExpiry = MembershipExpiry.AddDays(days);
        }

        LastModifiedAt = DateTime.UtcNow;
        AddDomainEvent(new MembershipRenewedEvent(Id, MembershipExpiry));
    }

    public void AddFavorite(Guid bookId)
    {
        if (_favorites.Any(f => f.BookId == bookId)) return;
        _favorites.Add(new MemberManagement.Entities.MemberFavorite(Guid.NewGuid(), Id, bookId));
        LastModifiedAt = DateTime.UtcNow;
    }

    public void RemoveFavorite(Guid bookId)
    {
        var favorite = _favorites.FirstOrDefault(f => f.BookId == bookId);
        if (favorite != null)
        {
            _favorites.Remove(favorite);
            LastModifiedAt = DateTime.UtcNow;
        }
    }

    // Checks if the member can borrow a book (not suspended, not overdue-blocked).
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
        Ensure.Against(string.IsNullOrWhiteSpace(name), "First name cannot be empty.", "MEMBER_FIRSTNAME_EMPTY");
        Ensure.Against(name.Length > MemberConsts.MaxFirstNameLength, $"First name cannot exceed {MemberConsts.MaxFirstNameLength} characters.", "MEMBER_FIRSTNAME_TOO_LONG");
        FirstName = name.Trim();
    }

    private void SetLastName(string name)
    {
        Ensure.Against(string.IsNullOrWhiteSpace(name), "Last name cannot be empty.", "MEMBER_LASTNAME_EMPTY");
        Ensure.Against(name.Length > MemberConsts.MaxLastNameLength, $"Last name cannot exceed {MemberConsts.MaxLastNameLength} characters.", "MEMBER_LASTNAME_TOO_LONG");
        LastName = name.Trim();
    }

    private void SetEmail(string email)
    {
        Ensure.Against(string.IsNullOrWhiteSpace(email), "Email cannot be empty.", "MEMBER_EMAIL_EMPTY");
        Ensure.Against(email.Length > MemberConsts.MaxEmailLength, $"Email cannot exceed {MemberConsts.MaxEmailLength} characters.", "MEMBER_EMAIL_TOO_LONG");
        Email = email.Trim().ToLowerInvariant();
    }

    private void SetPhone(string phone)
    {
        Ensure.Against(string.IsNullOrWhiteSpace(phone), "Phone cannot be empty.", "MEMBER_PHONE_EMPTY");
        Ensure.Against(phone.Length > MemberConsts.MaxPhoneLength, $"Phone number cannot exceed {MemberConsts.MaxPhoneLength} characters.", "MEMBER_PHONE_TOO_LONG");
        Phone = phone.Trim();
    }
}


