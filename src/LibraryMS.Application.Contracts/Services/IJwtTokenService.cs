using LibraryMS.Domain.IdentityManagement;
using LibraryMS.Domain.IdentityManagement.AggregateRoots;
using LibraryMS.Domain.IdentityManagement.Entities;
using LibraryMS.Domain.IdentityManagement.Services;
using System.Security.Claims;

namespace LibraryMS.Application.Contracts.Services;

// Interface for JWT token service — implemented in Infrastructure.
public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAt) GenerateAccessToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}

