using System;

namespace LibraryMS.Domain.IdentityManagement;

public sealed class RefreshTokenManager
{
    public RefreshToken Create(Guid userId, string token, string? createdByIp = null)
    {
        // Centralized policy for token expiry (e.g. 7 days)
        return new RefreshToken(
            Guid.NewGuid(),
            userId,
            token,
            DateTime.UtcNow.AddDays(7),
            createdByIp
        );
    }
}
