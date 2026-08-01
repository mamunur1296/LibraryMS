using LibraryMS.Domain.BookManagement.AggregateRoots;
using LibraryMS.EntityFrameworkCore;

namespace LibraryMS.DbMigrator.Seeders;

public class BookSeeder : IDataSeeder
{
    public async Task SeedAsync(LibraryDbContext dbContext, CancellationToken cancellationToken)
    {
        var mainBranchId = Guid.Parse("11110000-0000-0000-0000-000000000001");
        var downtownBranchId = Guid.Parse("22220000-0000-0000-0000-000000000002");

        var books = new List<Book>();

        var b1 = new Book(Guid.NewGuid(), "Clean Code", "9780132350884", "A handbook of agile software craftsmanship", 2008, CategorySeeder.ProgId, AuthorSeeder.UncleBobId, "English");
        b1.AddCopy(mainBranchId); b1.AddCopy(downtownBranchId); books.Add(b1);

        var b2 = new Book(Guid.NewGuid(), "Clean Architecture", "9780134494166", "A craftsman's guide to software structure and design", 2017, CategorySeeder.SeId, AuthorSeeder.UncleBobId, "English");
        b2.AddCopy(mainBranchId); b2.AddCopy(downtownBranchId); books.Add(b2);

        var b3 = new Book(Guid.NewGuid(), "Refactoring", "9780134757599", "Improving the design of existing code, 2nd Edition", 2018, CategorySeeder.SeId, AuthorSeeder.FowlerId, "English");
        b3.AddCopy(mainBranchId); b3.AddCopy(downtownBranchId); books.Add(b3);

        var b4 = new Book(Guid.NewGuid(), "Patterns of Enterprise Application Architecture", "9780321127426", "Catalog of patterns for enterprise application development", 2002, CategorySeeder.DbId, AuthorSeeder.FowlerId, "English");
        b4.AddCopy(mainBranchId); b4.AddCopy(downtownBranchId); books.Add(b4);

        var b5 = new Book(Guid.NewGuid(), "Domain-Driven Design", "9780321125217", "Tackling complexity in the heart of software", 2003, CategorySeeder.SeId, AuthorSeeder.EvansId, "English");
        b5.AddCopy(mainBranchId); b5.AddCopy(downtownBranchId); books.Add(b5);

        var b6 = new Book(Guid.NewGuid(), "Domain-Driven Design Reference", "9781457501197", "Definitions and pattern summaries for DDD", 2014, CategorySeeder.CsId, AuthorSeeder.EvansId, "English");
        b6.AddCopy(mainBranchId); b6.AddCopy(downtownBranchId); books.Add(b6);

        var b7 = new Book(Guid.NewGuid(), "The Pragmatic Programmer", "9780135957059", "Your journey to mastery, 20th Anniversary Edition", 2019, CategorySeeder.ProgId, AuthorSeeder.HuntId, "English");
        b7.AddCopy(mainBranchId); b7.AddCopy(downtownBranchId); books.Add(b7);

        var b8 = new Book(Guid.NewGuid(), "Programming Ruby", "9780974514055", "The pragmatic programmer's guide to Ruby", 2004, CategorySeeder.ProgId, AuthorSeeder.HuntId, "English");
        b8.AddCopy(mainBranchId); b8.AddCopy(downtownBranchId); books.Add(b8);

        var b9 = new Book(Guid.NewGuid(), "The Art of Computer Programming, Vol. 1", "9780201896831", "Fundamental algorithms", 1997, CategorySeeder.AlgoId, AuthorSeeder.KnuthId, "English");
        b9.AddCopy(mainBranchId); b9.AddCopy(downtownBranchId); books.Add(b9);

        var b10 = new Book(Guid.NewGuid(), "The Art of Computer Programming, Vol. 2", "9780201896848", "Seminumerical algorithms", 1997, CategorySeeder.AlgoId, AuthorSeeder.KnuthId, "English");
        b10.AddCopy(mainBranchId); b10.AddCopy(downtownBranchId); books.Add(b10);

        dbContext.Books.AddRange(books);
    }
}
