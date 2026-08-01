using LibraryMS.EntityFrameworkCore;

namespace LibraryMS.DbMigrator.Seeders;

public interface IDataSeeder
{
    Task SeedAsync(LibraryDbContext dbContext, CancellationToken cancellationToken);
}
