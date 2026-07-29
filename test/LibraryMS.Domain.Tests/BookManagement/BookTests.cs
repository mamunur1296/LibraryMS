using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.BookManagement.AggregateRoots;
using LibraryMS.Domain.BookManagement.Entities;
using LibraryMS.Domain.BookManagement.Services;
using LibraryMS.Domain.BookManagement.VOs;
using LibraryMS.Domain.Shared.Exceptions;
using FluentAssertions;
using System;
using Xunit;

namespace LibraryMS.Domain.Tests.BookManagement;

public class BookTests
{
    private static Book CreateBook(string title = "Clean Code", string isbn = "9780132350884")
    {
        var categoryId = Guid.NewGuid();
        var authorId = Guid.NewGuid();
        return new Book(
            Guid.NewGuid(),
            title,
            isbn,
            "Software Legend",
            2008,
            categoryId,
            authorId,
            "English");
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

        var copy = book.AddCopy(branchId);

        book.TotalCopies.Should().Be(1);
        book.AvailableCopies.Should().Be(1);
        copy.Status.Should().Be(LibraryMS.Domain.Shared.Enums.CopyStatus.Available);
    }

    [Fact]
    public void BorrowCopy_ShouldReduceAvailableCopies()
    {
        var book = CreateBook();
        var branchId = Guid.NewGuid();
        var copy = book.AddCopy(branchId);

        book.BorrowCopy(copy.Id);

        book.AvailableCopies.Should().Be(0);
        copy.Status.Should().Be(LibraryMS.Domain.Shared.Enums.CopyStatus.Borrowed);
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

