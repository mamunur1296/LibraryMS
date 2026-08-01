using LibraryMS.Domain.BookManagement.AggregateRoots;
using LibraryMS.Domain.BookManagement.Entities;
using LibraryMS.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryMS.DbMigrator.Seeders;

public class AuthorSeeder : IDataSeeder
{
    public static readonly Guid UncleBobId = Guid.Parse("10000000-0000-0000-0000-000000000001");
    public static readonly Guid FowlerId = Guid.Parse("10000000-0000-0000-0000-000000000002");
    public static readonly Guid EvansId = Guid.Parse("10000000-0000-0000-0000-000000000003");
    public static readonly Guid HuntId = Guid.Parse("10000000-0000-0000-0000-000000000004");
    public static readonly Guid KnuthId = Guid.Parse("10000000-0000-0000-0000-000000000005");

    public async Task SeedAsync(LibraryDbContext dbContext, CancellationToken cancellationToken)
    {
        var uncleBob = new Author(UncleBobId, "Robert C. Martin", "Known as Uncle Bob, author of Clean Code series");
        var fowler = new Author(FowlerId, "Martin Fowler", "Chief Scientist at ThoughtWorks, expert in refactoring");
        var evans = new Author(EvansId, "Eric Evans", "Author of Domain-Driven Design");
        var hunt = new Author(HuntId, "Andrew Hunt", "Co-author of The Pragmatic Programmer");
        var knuth = new Author(KnuthId, "Donald E. Knuth", "Renowned computer scientist, author of TAOCP");
        
        var authors = new[] { uncleBob, fowler, evans, hunt, knuth };
        dbContext.Authors.AddRange(authors);
    }
}
