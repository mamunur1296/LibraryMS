using LibraryMS.Domain.Common;

namespace LibraryMS.Domain.IdentityManagement.Entities;

// Refresh token entity for JWT rotation.
public sealed class RefreshToken : Entity<Guid>
{
    public Guid UserId { get; private set; }
    public string Token { get; private set; } = default!;
    public DateTime ExpiresAt { get; private set; }

    public DateTime? RevokedAt { get; private set; }
    public string? CreatedByIp { get; private set; }
    public bool IsUsed { get; private set; }

    private RefreshToken() { }

    public RefreshToken(Guid id, Guid userId, string token, DateTime expiresAt, string? createdByIp)
        : base(id)
    {
        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
        CreatedAt = DateTime.UtcNow;
        CreatedByIp = createdByIp;
    }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt.HasValue;
    public bool IsActive => !IsRevoked && !IsExpired && !IsUsed;

    public void MarkAsUsed()
    {
        IsUsed = true;
    }

    public void Revoke()
    {
        RevokedAt = DateTime.UtcNow;
    }
}

