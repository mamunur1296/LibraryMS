using LibraryMS.Domain.IdentityManagement;
using System.Security.Claims;

namespace LibraryMS.Application.Contracts.Services;

// Interface for JWT token service — implemented in Infrastructure.
public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAt) GenerateAccessToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}
