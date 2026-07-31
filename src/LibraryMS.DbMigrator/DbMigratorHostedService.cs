using LibraryMS.Application.Contracts.Services;
using LibraryMS.Domain.BookManagement.AggregateRoots;
using LibraryMS.Domain.BookManagement.Entities;
using LibraryMS.Domain.BranchManagement.AggregateRoots;
using LibraryMS.Domain.IdentityManagement.AggregateRoots;
using LibraryMS.Domain.MemberManagement.AggregateRoots;
using LibraryMS.Domain.Shared.Enums;
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
        if (await dbContext.Branches.AnyAsync(cancellationToken))
        {
            _logger.LogInformation("Database already seeded. Skipping.");
            return;
        }

        _logger.LogInformation("Seeding comprehensive test data...");

        // ============================================================
        // 1. Branches (5)
        // ============================================================
        var mainBranch = new Branch(Guid.NewGuid(), "Main Library", "123 Central Ave, New York, NY 10001", "555-0101", "main@library.com");
        var downtownBranch = new Branch(Guid.NewGuid(), "Downtown Branch", "456 Market St, San Francisco, CA 94105", "555-0102", "downtown@library.com");
        var universityBranch = new Branch(Guid.NewGuid(), "University Branch", "789 College Ave, Chicago, IL 60616", "555-0103", "university@library.com");
        var riversideBranch = new Branch(Guid.NewGuid(), "Riverside Branch", "321 River Rd, Miami, FL 33101", "555-0104", "riverside@library.com");
        var eastsideBranch = new Branch(Guid.NewGuid(), "Eastside Branch", "654 Park Blvd, Boston, MA 02101", "555-0105", "eastside@library.com");
        var branches = new[] { mainBranch, downtownBranch, universityBranch, riversideBranch, eastsideBranch };
        dbContext.Branches.AddRange(branches);

        // ============================================================
        // 2. Authors (5)
        // ============================================================
        var uncleBob = new Author(Guid.NewGuid(), "Robert C. Martin", "Known as Uncle Bob, author of Clean Code series");
        var fowler = new Author(Guid.NewGuid(), "Martin Fowler", "Chief Scientist at ThoughtWorks, expert in refactoring");
        var evans = new Author(Guid.NewGuid(), "Eric Evans", "Author of Domain-Driven Design");
        var hunt = new Author(Guid.NewGuid(), "Andrew Hunt", "Co-author of The Pragmatic Programmer");
        var knuth = new Author(Guid.NewGuid(), "Donald E. Knuth", "Renowned computer scientist, author of TAOCP");
        var authors = new[] { uncleBob, fowler, evans, hunt, knuth };
        dbContext.Authors.AddRange(authors);

        // ============================================================
        // 3. Categories (5)
        // ============================================================
        var cs = new Category(Guid.NewGuid(), "Computer Science", "Core computer science concepts");
        var se = new Category(Guid.NewGuid(), "Software Engineering", "Software design and architecture patterns");
        var prog = new Category(Guid.NewGuid(), "Programming", "Programming languages and best practices");
        var db = new Category(Guid.NewGuid(), "Database", "Database design, management, and architecture");
        var algo = new Category(Guid.NewGuid(), "Algorithms", "Algorithm analysis, design, and optimization");
        var categories = new[] { cs, se, prog, db, algo };
        dbContext.Categories.AddRange(categories);

        // ============================================================
        // 4. Books (10) + BookCopies (20 — 2 per book)
        //    Copy 1 → Main Library, Copy 2 → Downtown Branch
        // ============================================================
        var books = new List<Book>();

        var b1 = new Book(Guid.NewGuid(), "Clean Code", "9780132350884", "A handbook of agile software craftsmanship", 2008, prog.Id, uncleBob.Id, "English");
        b1.AddCopy(mainBranch.Id); b1.AddCopy(downtownBranch.Id); books.Add(b1);

        var b2 = new Book(Guid.NewGuid(), "Clean Architecture", "9780134494166", "A craftsman's guide to software structure and design", 2017, se.Id, uncleBob.Id, "English");
        b2.AddCopy(mainBranch.Id); b2.AddCopy(downtownBranch.Id); books.Add(b2);

        var b3 = new Book(Guid.NewGuid(), "Refactoring", "9780134757599", "Improving the design of existing code, 2nd Edition", 2018, se.Id, fowler.Id, "English");
        b3.AddCopy(mainBranch.Id); b3.AddCopy(downtownBranch.Id); books.Add(b3);

        var b4 = new Book(Guid.NewGuid(), "Patterns of Enterprise Application Architecture", "9780321127426", "Catalog of patterns for enterprise application development", 2002, db.Id, fowler.Id, "English");
        b4.AddCopy(mainBranch.Id); b4.AddCopy(downtownBranch.Id); books.Add(b4);

        var b5 = new Book(Guid.NewGuid(), "Domain-Driven Design", "9780321125217", "Tackling complexity in the heart of software", 2003, se.Id, evans.Id, "English");
        b5.AddCopy(mainBranch.Id); b5.AddCopy(downtownBranch.Id); books.Add(b5);

        var b6 = new Book(Guid.NewGuid(), "Domain-Driven Design Reference", "9781457501197", "Definitions and pattern summaries for DDD", 2014, cs.Id, evans.Id, "English");
        b6.AddCopy(mainBranch.Id); b6.AddCopy(downtownBranch.Id); books.Add(b6);

        var b7 = new Book(Guid.NewGuid(), "The Pragmatic Programmer", "9780135957059", "Your journey to mastery, 20th Anniversary Edition", 2019, prog.Id, hunt.Id, "English");
        b7.AddCopy(mainBranch.Id); b7.AddCopy(downtownBranch.Id); books.Add(b7);

        var b8 = new Book(Guid.NewGuid(), "Programming Ruby", "9780974514055", "The pragmatic programmer's guide to Ruby", 2004, prog.Id, hunt.Id, "English");
        b8.AddCopy(mainBranch.Id); b8.AddCopy(downtownBranch.Id); books.Add(b8);

        var b9 = new Book(Guid.NewGuid(), "The Art of Computer Programming, Vol. 1", "9780201896831", "Fundamental algorithms", 1997, algo.Id, knuth.Id, "English");
        b9.AddCopy(mainBranch.Id); b9.AddCopy(downtownBranch.Id); books.Add(b9);

        var b10 = new Book(Guid.NewGuid(), "The Art of Computer Programming, Vol. 2", "9780201896848", "Seminumerical algorithms", 1997, algo.Id, knuth.Id, "English");
        b10.AddCopy(mainBranch.Id); b10.AddCopy(downtownBranch.Id); books.Add(b10);

        dbContext.Books.AddRange(books);

        // ============================================================
        // 5. Members (3)
        // ============================================================
        var memberJohn = new Member(Guid.NewGuid(), "John", "Doe", "john.doe@email.com", "555-1001", "LIB-2026-00001", "123 Main St, New York, NY");
        var memberJane = new Member(Guid.NewGuid(), "Jane", "Smith", "jane.smith@email.com", "555-1002", "LIB-2026-00002", "456 Oak Ave, San Francisco, CA");
        var memberBob = new Member(Guid.NewGuid(), "Bob", "Johnson", "bob.johnson@email.com", "555-1003", "LIB-2026-00003", "789 Pine Rd, Chicago, IL");
        var members = new[] { memberJohn, memberJane, memberBob };
        dbContext.Members.AddRange(members);

        // ============================================================
        // 6. Users (3) — one per role
        //    member user is linked to John Doe's Member record
        // ============================================================
        var (adminHash, adminSalt) = passwordHasher.Hash("Admin123!");
        var (libHash, libSalt) = passwordHasher.Hash("Librarian123!");
        var (memberHash, memberSalt) = passwordHasher.Hash("Member123!");

        var adminUser = new User(Guid.NewGuid(), "admin", "admin@library.com", adminHash, adminSalt, UserRole.Admin);
        var libUser = new User(Guid.NewGuid(), "librarian", "librarian@library.com", libHash, libSalt, UserRole.Librarian);
        var memberUser = new User(Guid.NewGuid(), "member", "member@library.com", memberHash, memberSalt, UserRole.Member, memberJohn.Id);
        dbContext.Users.AddRange(adminUser, libUser, memberUser);

        // ============================================================
        // Save all seed data
        // ============================================================
        await dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Seed complete: {BranchCount} branches, {AuthorCount} authors, {CategoryCount} categories, " +
            "{BookCount} books ({CopyCount} copies), {MemberCount} members, {UserCount} users",
            branches.Length, authors.Length, categories.Length,
            books.Count, books.Sum(b => b.TotalCopies), members.Length, 3);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
