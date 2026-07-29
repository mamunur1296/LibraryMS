using LibraryMS.EntityFrameworkCore.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace LibraryMS.EntityFrameworkCore;

// EF Core Design-Time DbContext Factory.
// Used by 'dotnet ef migrations' commands to safely instantiate the DbContext 
// without needing the full web host DI container at design time.
public sealed class LibraryDbContextFactory : IDesignTimeDbContextFactory<LibraryDbContext>
{
    public LibraryDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "src", "LibraryMS.HttpApi.Host"))
            .AddJsonFile("appsettings.json")
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<LibraryDbContext>();

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        optionsBuilder.UseNpgsql(connectionString);

        // Instantiate interceptors required by LibraryDbContext constructor
        var auditableInterceptor = new AuditableEntityInterceptor();
        var domainEventInterceptor = new DomainEventToOutboxInterceptor();

        return new LibraryDbContext(
            optionsBuilder.Options,
            auditableInterceptor,
            domainEventInterceptor);
    }
}
