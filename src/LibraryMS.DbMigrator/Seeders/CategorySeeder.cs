using LibraryMS.Domain.BookManagement.AggregateRoots;
using LibraryMS.Domain.BookManagement.Entities;
using LibraryMS.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryMS.DbMigrator.Seeders;

public class CategorySeeder : IDataSeeder
{
    public static readonly Guid CsId = Guid.Parse("20000000-0000-0000-0000-000000000001");
    public static readonly Guid SeId = Guid.Parse("20000000-0000-0000-0000-000000000002");
    public static readonly Guid ProgId = Guid.Parse("20000000-0000-0000-0000-000000000003");
    public static readonly Guid DbId = Guid.Parse("20000000-0000-0000-0000-000000000004");
    public static readonly Guid AlgoId = Guid.Parse("20000000-0000-0000-0000-000000000005");

    public async Task SeedAsync(LibraryDbContext dbContext, CancellationToken cancellationToken)
    {
        var cs = new Category(CsId, "Computer Science", "Core computer science concepts");
        var se = new Category(SeId, "Software Engineering", "Software design and architecture patterns");
        var prog = new Category(ProgId, "Programming", "Programming languages and best practices");
        var db = new Category(DbId, "Database", "Database design, management, and architecture");
        var algo = new Category(AlgoId, "Algorithms", "Algorithm analysis, design, and optimization");
        
        var categories = new[] { cs, se, prog, db, algo };
        dbContext.Categories.AddRange(categories);
    }
}
