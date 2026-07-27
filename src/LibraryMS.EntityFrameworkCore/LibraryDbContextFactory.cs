using LibraryMS.EntityFrameworkCore.Interceptors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LibraryMS.EntityFrameworkCore;

/// <summary>
/// EF Core Design-Time DbContext Factory.
/// Used by 'dotnet ef migrations' commands to safely instantiate the DbContext 
/// without needing the full web host DI container at design time.
/// </summary>
public sealed class LibraryDbContextFactory : IDesignTimeDbContextFactory<LibraryDbContext>
{
    public LibraryDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<LibraryDbContext>();
        
        // Dummy/development connection string for migrations generation
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=LibraryMS;Username=postgres;Password=2025");

        // Instantiate interceptors required by LibraryDbContext constructor
        var auditableInterceptor = new AuditableEntityInterceptor();
        var domainEventInterceptor = new DomainEventToOutboxInterceptor();

        return new LibraryDbContext(
            optionsBuilder.Options,
            auditableInterceptor,
            domainEventInterceptor);
    }
}
