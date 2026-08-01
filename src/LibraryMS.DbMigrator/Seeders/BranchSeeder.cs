using LibraryMS.Domain.BranchManagement.AggregateRoots;
using LibraryMS.EntityFrameworkCore;

namespace LibraryMS.DbMigrator.Seeders;

public class BranchSeeder : IDataSeeder
{
    public static readonly Guid MainBranchId = Guid.Parse("11110000-0000-0000-0000-000000000001");
    public static readonly Guid DowntownBranchId = Guid.Parse("22220000-0000-0000-0000-000000000002");
    
    public async Task SeedAsync(LibraryDbContext dbContext, CancellationToken cancellationToken)
    {
        var mainBranch = new Branch(MainBranchId, "Main Library", "123 Central Ave, New York, NY 10001", "555-0101", "main@library.com");
        var downtownBranch = new Branch(DowntownBranchId, "Downtown Branch", "456 Market St, San Francisco, CA 94105", "555-0102", "downtown@library.com");
        var universityBranch = new Branch(Guid.Parse("33330000-0000-0000-0000-000000000003"), "University Branch", "789 College Ave, Chicago, IL 60616", "555-0103", "university@library.com");
        var riversideBranch = new Branch(Guid.Parse("44440000-0000-0000-0000-000000000004"), "Riverside Branch", "321 River Rd, Miami, FL 33101", "555-0104", "riverside@library.com");
        var eastsideBranch = new Branch(Guid.Parse("55550000-0000-0000-0000-000000000005"), "Eastside Branch", "654 Park Blvd, Boston, MA 02101", "555-0105", "eastside@library.com");

        var branches = new[] { mainBranch, downtownBranch, universityBranch, riversideBranch, eastsideBranch };
        dbContext.Branches.AddRange(branches);
    }
}
