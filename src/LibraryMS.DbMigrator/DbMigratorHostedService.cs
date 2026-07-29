using LibraryMS.Domain.BranchManagement;
using LibraryMS.Domain.BranchManagement.AggregateRoots;
using LibraryMS.Domain.BranchManagement.Services;
using LibraryMS.Domain.IdentityManagement;
using LibraryMS.Domain.IdentityManagement.AggregateRoots;
using LibraryMS.Domain.IdentityManagement.Entities;
using LibraryMS.Domain.IdentityManagement.Services;
using LibraryMS.Domain.Shared.Enums;
using LibraryMS.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using LibraryMS.Application.Contracts.Services;

namespace LibraryMS.DbMigrator;

public class DbMigratorHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DbMigratorHostedService> _logger;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;

    public DbMigratorHostedService(
        IServiceProvider serviceProvider,
        ILogger<DbMigratorHostedService> logger,
        IHostApplicationLifetime hostApplicationLifetime)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _hostApplicationLifetime = hostApplicationLifetime;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting database migration...");

        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        try
        {
            await dbContext.Database.MigrateAsync(cancellationToken);
            _logger.LogInformation("Migration applied successfully.");

            await SeedDataAsync(dbContext, passwordHasher, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during database migration.");
        }
        finally
        {
            _hostApplicationLifetime.StopApplication();
        }
    }

    private async Task SeedDataAsync(LibraryDbContext dbContext, IPasswordHasher passwordHasher, CancellationToken cancellationToken)
    {
        if (!await dbContext.Branches.AnyAsync(cancellationToken))
        {
            _logger.LogInformation("Seeding default branch...");
            var branch = new Branch(Guid.NewGuid(), "Main Library", "123 Central Ave, NY", "555-1234", "main@library.com");
            dbContext.Branches.Add(branch);
        }

        if (!await dbContext.Users.AnyAsync(cancellationToken))
        {
            _logger.LogInformation("Seeding admin user...");
            var (hash, salt) = passwordHasher.Hash("Admin123!");
            var admin = new User(Guid.NewGuid(), "admin", "admin@library.com", hash, salt, UserRole.Admin);
            dbContext.Users.Add(admin);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Data seeding completed.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

