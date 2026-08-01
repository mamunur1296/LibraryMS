using FluentAssertions;
using LibraryMS.Application.Books;
using LibraryMS.Application.Contracts.Books;
using LibraryMS.Application.Mapping;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.BookManagement.AggregateRoots;
using LibraryMS.Domain.Shared.Exceptions;
using LibraryMS.TestBase;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LibraryMS.Application.Tests.Books;

public class GetBookByIdQueryHandlerTests
{
    private readonly Mock<IBookRepository> _bookRepoMock;
    
    private readonly GetBookByIdQueryHandler _handler;

    public GetBookByIdQueryHandlerTests()
    {
        _bookRepoMock = new Mock<IBookRepository>();

        _handler = new GetBookByIdQueryHandler(_bookRepoMock.Object);
    }

    [Fact]
    public async Task Handle_BookExists_ReturnsBookDto()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var book = TestDataFactory.CreateBook();
        _bookRepoMock.Setup(x => x.GetByIdWithCopiesAsync(bookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        var query = new GetBookByIdQuery(bookId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be(book.Title);
        result.ISBN.Should().Be(book.ISBN.Value);
    }

    [Fact]
    public async Task Handle_BookDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        _bookRepoMock.Setup(x => x.GetByIdWithCopiesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Book?)null);

        var query = new GetBookByIdQuery(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}
