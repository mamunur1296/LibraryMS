using LibraryMS.Domain.IdentityManagement;

namespace LibraryMS.Application.Auth;

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
    System.Security.Claims.ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}
