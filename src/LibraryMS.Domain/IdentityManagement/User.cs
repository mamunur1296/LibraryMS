using LibraryMS.Domain.Common;
using LibraryMS.Domain.IdentityManagement.Events;
using LibraryMS.Domain.Shared.Enums;
using LibraryMS.Domain.Shared.Exceptions;

namespace LibraryMS.Domain.IdentityManagement;

/// <summary>
/// User — system user for authentication (Admin/Librarian/Member roles).
/// </summary>
public sealed class User : AggregateRoot<Guid>
{
    public string Username { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public string PasswordSalt { get; private set; } = default!;
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; } = true;
    public Guid? MemberId { get; private set; }  // linked to Member if role=Member

    public DateTime? LastLoginAt { get; private set; }

    private User() { }

    internal User(Guid id, string username, string email,
        string passwordHash, string salt, UserRole role, Guid? memberId = null)
        : base(id)
    {
        SetUsername(username);
        SetEmail(email);
        PasswordHash = passwordHash;
        PasswordSalt = salt;
        Role = role;
        MemberId = memberId;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;

        AddDomainEvent(new UserCreatedEvent(id, username, email, role));
    }

    internal void UpdatePassword(string newHash, string newSalt)
    {
        PasswordHash = newHash;
        PasswordSalt = newSalt;
    }

    internal void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }

    internal void Deactivate()
    {
        if (!IsActive)
            throw new DomainException("User is already inactive.", "USER_ALREADY_INACTIVE");
        IsActive = false;
    }

    internal void Activate()
    {
        if (IsActive)
            throw new DomainException("User is already active.", "USER_ALREADY_ACTIVE");
        IsActive = true;
    }

    internal void ChangeRole(UserRole newRole)
    {
        Role = newRole;
    }

    private void SetUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new DomainException("Username cannot be empty.", "USER_USERNAME_EMPTY");
        if (username.Length < 3 || username.Length > 50)
            throw new DomainException("Username must be between 3 and 50 characters.", "USER_USERNAME_LENGTH");
        Username = username.Trim().ToLowerInvariant();
    }

    private void SetEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email cannot be empty.", "USER_EMAIL_EMPTY");
        Email = email.Trim().ToLowerInvariant();
    }
}

