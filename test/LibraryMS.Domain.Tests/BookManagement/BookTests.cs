using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.BookManagement.VOs;
using LibraryMS.Domain.Shared.Exceptions;
using FluentAssertions;

namespace LibraryMS.Domain.Tests.BookManagement;

public class BookTests
{
    private static Book CreateBook(string title = "Clean Code", string isbn = "9780132350884")
    {
        var ctor = typeof(Book).GetConstructors(
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .First(c => c.GetParameters().Length == 7);

        return (Book)ctor.Invoke([
            Guid.NewGuid(), title, isbn, "A description", 2008, Guid.NewGuid(), Guid.NewGuid(), "English"
        ]);
    }

    [Fact]
    public void Constructor_ShouldRaiseBookCreatedEvent()
    {
        var book = CreateBook();
        book.DomainEvents.Should().ContainSingle(e => e is LibraryMS.Domain.BookManagement.Events.BookCreatedEvent);
    }

    [Fact]
    public void AddCopy_ShouldIncreaseTotalCopies()
    {
        var book = CreateBook();
        var branchId = Guid.NewGuid();

        var addCopyMethod = typeof(Book).GetMethod("AddCopy",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        addCopyMethod!.Invoke(book, [branchId]);

        book.TotalCopies.Should().Be(1);
        book.AvailableCopies.Should().Be(1);
    }

    [Fact]
    public void BorrowCopy_ShouldReduceAvailableCopies()
    {
        var book = CreateBook();
        var branchId = Guid.NewGuid();

        var addCopyMethod = typeof(Book).GetMethod("AddCopy",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var copy = (BookCopy)addCopyMethod!.Invoke(book, [branchId])!;

        var borrowMethod = typeof(Book).GetMethod("BorrowCopy",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        borrowMethod!.Invoke(book, [copy.Id]);

        book.AvailableCopies.Should().Be(0);
    }
}

public class ISBNTests
{
    [Theory]
    [InlineData("9780132350884")]  // Valid ISBN-13
    [InlineData("0132350882")]     // Valid ISBN-10
    public void Create_WithValidISBN_ShouldSucceed(string isbn)
    {
        var act = () => ISBN.Create(isbn);
        act.Should().NotThrow();
    }

    [Theory]
    [InlineData("1234")]           // Too short
    [InlineData("12345678901234")] // Too long (14 digits)
    [InlineData("978-ABC-XXXXX")]  // Letters
    public void Create_WithInvalidISBN_ShouldThrow(string isbn)
    {
        var act = () => ISBN.Create(isbn);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithHyphens_ShouldNormalize()
    {
        var isbn = ISBN.Create("978-0-13-235088-4");
        isbn.Value.Should().Be("9780132350884");
    }

    [Fact]
    public void TwoISBNs_WithSameValue_ShouldBeEqual()
    {
        var a = ISBN.Create("9780132350884");
        var b = ISBN.Create("9780132350884");
        a.Should().Be(b);
    }
}
