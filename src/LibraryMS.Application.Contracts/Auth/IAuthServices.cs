using LibraryMS.Domain.IdentityManagement;
using System.Security.Claims;

namespace LibraryMS.Application.Contracts.Auth;

/// <summary>Interface for password hashing — implemented in Infrastructure.</summary>
public interface IPasswordHasher
{
    (string Hash, string Salt) Hash(string password);
    bool Verify(string password, string hash, string salt);
}

/// <summary>Interface for JWT token service — implemented in Infrastructure.</summary>
public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAt) GenerateAccessToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}
