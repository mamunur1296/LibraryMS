using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace LibraryMS.EntityFrameworkCore;

public sealed class LibraryDbContextFactory : IDesignTimeDbContextFactory<LibraryDbContext>
{
    public LibraryDbContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory();
        if (!File.Exists(Path.Combine(basePath, "appsettings.json")))
        {
            var candidate = Path.Combine(basePath, "src", "LibraryMS.HttpApi.Host");
            if (Directory.Exists(candidate))
            {
                basePath = candidate;
            }
            else
            {
                candidate = Path.Combine(basePath, "..", "LibraryMS.HttpApi.Host");
                if (Directory.Exists(candidate))
                {
                    basePath = Path.GetFullPath(candidate);
                }
            }
        }

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json")
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<LibraryDbContext>();

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        optionsBuilder.UseNpgsql(connectionString);

        return new LibraryDbContext(optionsBuilder.Options);
    }
}
