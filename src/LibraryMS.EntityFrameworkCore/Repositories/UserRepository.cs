using LibraryMS.Domain.IdentityManagement;
using LibraryMS.Domain.IdentityManagement.AggregateRoots;
using LibraryMS.Domain.IdentityManagement.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryMS.EntityFrameworkCore.Repositories;

public sealed class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(LibraryDbContext dbContext) : base(dbContext) { }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower(), cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower(), cancellationToken);
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await DbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);
    }

    public async Task AddRefreshTokenAsync(RefreshToken token, CancellationToken cancellationToken = default)
    {
        DbContext.RefreshTokens.Add(token);
        await DbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RevokeRefreshTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        var rt = await GetRefreshTokenAsync(token, cancellationToken);
        if (rt is not null)
        {
            rt.Revoke();
            DbContext.RefreshTokens.Update(rt);
            await DbContext.SaveChangesAsync(cancellationToken);
        }
    }
    public async Task RevokeAllUserRefreshTokensAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var tokens = await DbContext.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.IsActive)
            .ToListAsync(cancellationToken);

        foreach (var rt in tokens)
        {
            rt.Revoke();
            DbContext.RefreshTokens.Update(rt);
        }

        if (tokens.Count > 0)
            await DbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(u => u.Username.ToLower() == username.ToLower(), cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(u => u.Email.ToLower() == email.ToLower(), cancellationToken);
    }

    public async Task<List<User>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.ToListAsync(cancellationToken);
    }
}

