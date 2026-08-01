using FluentAssertions;
using LibraryMS.Application.Books;
using LibraryMS.Application.Contracts.Books;
using LibraryMS.Application.Contracts.Common;
using LibraryMS.Application.Contracts.DTOs.Book;
using LibraryMS.Application.Contracts.Services;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.BookManagement.Entities;
using LibraryMS.TestBase;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LibraryMS.Application.Tests.Books;

public class SearchBooksQueryHandlerTests
{
    private readonly Mock<IBookRepository> _bookRepoMock;
    private readonly Mock<ICacheService> _cacheMock;
    private readonly Mock<ILogger<SearchBooksQueryHandler>> _loggerMock;
    private readonly SearchBooksQueryHandler _handler;

    public SearchBooksQueryHandlerTests()
    {
        _bookRepoMock = new Mock<IBookRepository>();
        _cacheMock = new Mock<ICacheService>();
        _loggerMock = new Mock<ILogger<SearchBooksQueryHandler>>();

        _handler = new SearchBooksQueryHandler(
            _bookRepoMock.Object,
            _cacheMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_CachedResultExists_ReturnsCachedResult()
    {
        // Arrange
        var pagedResult = PagedResult<BookDto>.Create(new List<BookDto>(), 0, 1, 10);
        _cacheMock.Setup(c => c.GetAsync<PagedResult<BookDto>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        var query = new SearchBooksQuery("Term", null, null, null, 1, 10);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(pagedResult);
        _bookRepoMock.Verify(r => r.SearchAsync(It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<Guid?>(), It.IsAny<Guid?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_NoCachedResult_SearchesAndCachesResult()
    {
        // Arrange
        var book = TestDataFactory.CreateBook();
        var author = new Author(book.AuthorId, "Author Name", "Bio");
        var category = new Category(book.CategoryId, "Category", "Desc");
        
        _cacheMock.Setup(c => c.GetAsync<PagedResult<BookDto>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PagedResult<BookDto>?)null);

        _bookRepoMock.Setup(r => r.SearchAsync("Term", null, null, null, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<LibraryMS.Domain.BookManagement.AggregateRoots.Book> { book }, 1));
            
        _bookRepoMock.Setup(r => r.GetAllAuthorsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Author> { author });
            
        _bookRepoMock.Setup(r => r.GetAllCategoriesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Category> { category });

        var query = new SearchBooksQuery("Term", null, null, null, 1, 10);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TotalCount.Should().Be(1);
        result.Items.Should().HaveCount(1);
        result.Items[0].Title.Should().Be(book.Title);
        
        _cacheMock.Verify(c => c.SetAsync(It.IsAny<string>(), result, It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
