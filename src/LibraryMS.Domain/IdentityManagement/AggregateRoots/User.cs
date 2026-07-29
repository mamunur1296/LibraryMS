using LibraryMS.Domain.Common;
using LibraryMS.Domain.IdentityManagement.Events;
using LibraryMS.Domain.Shared.Enums;
using LibraryMS.Domain.Shared.Guards;

namespace LibraryMS.Domain.IdentityManagement.AggregateRoots;

// User — system user for authentication (Admin/Librarian/Member roles).
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
        Ensure.Against(!IsActive, "User is already inactive.", "USER_ALREADY_INACTIVE");
        IsActive = false;
    }

    internal void Activate()
    {
        Ensure.Against(IsActive, "User is already active.", "USER_ALREADY_ACTIVE");
        IsActive = true;
    }

    internal void ChangeRole(UserRole newRole)
    {
        Role = newRole;
    }

    internal void ChangeUsername(string newUsername)
    {
        SetUsername(newUsername);
    }

    internal void ChangeEmail(string newEmail)
    {
        SetEmail(newEmail);
    }

    private void SetUsername(string username)
    {
        Ensure.Against(string.IsNullOrWhiteSpace(username), "Username cannot be empty.", "USER_USERNAME_EMPTY");
        Ensure.Against(username.Length < 3 || username.Length > 50, "Username must be between 3 and 50 characters.", "USER_USERNAME_LENGTH");
        Username = username.Trim().ToLowerInvariant();
    }

    private void SetEmail(string email)
    {
        Ensure.Against(string.IsNullOrWhiteSpace(email), "Email cannot be empty.", "USER_EMAIL_EMPTY");
        Email = email.Trim().ToLowerInvariant();
    }
}

