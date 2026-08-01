using LibraryMS.DbMigrator.Seeders;
using LibraryMS.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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

        try
        {
            await dbContext.Database.MigrateAsync(cancellationToken);
            _logger.LogInformation("Migration applied successfully.");

            await SeedDataAsync(scope.ServiceProvider, dbContext, cancellationToken);
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

    private async Task SeedDataAsync(IServiceProvider serviceProvider, LibraryDbContext dbContext, CancellationToken cancellationToken)
    {
        if (await dbContext.Branches.AnyAsync(cancellationToken))
        {
            _logger.LogInformation("Database already seeded. Skipping.");
            return;
        }

        _logger.LogInformation("Seeding comprehensive test data...");

        // Resolve and run all seeders
        var seeders = serviceProvider.GetServices<IDataSeeder>();
        foreach (var seeder in seeders)
        {
            _logger.LogInformation($"Running {seeder.GetType().Name}...");
            await seeder.SeedAsync(dbContext, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Seed complete.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
