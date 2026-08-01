using FluentAssertions;
using LibraryMS.Application.Books;
using LibraryMS.Application.Contracts.Books;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.BookManagement.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LibraryMS.Application.Tests.Books;

public class GetAllAuthorsQueryHandlerTests
{
    private readonly Mock<IBookRepository> _bookRepoMock;
    private readonly GetAllAuthorsQueryHandler _handler;

    public GetAllAuthorsQueryHandlerTests()
    {
        _bookRepoMock = new Mock<IBookRepository>();
        _handler = new GetAllAuthorsQueryHandler(_bookRepoMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsAllAuthors()
    {
        // Arrange
        var authors = new List<Author>
        {
            new Author(Guid.NewGuid(), "Author 1", "Bio 1"),
            new Author(Guid.NewGuid(), "Author 2", "Bio 2")
        };
        
        _bookRepoMock.Setup(r => r.GetAllAuthorsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(authors);
            
        var query = new GetAllAuthorsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result[0].Name.Should().Be("Author 1");
    }

    [Fact]
    public async Task Handle_NoAuthors_ReturnsEmptyList()
    {
        // Arrange
        _bookRepoMock.Setup(r => r.GetAllAuthorsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Author>());
            
        var query = new GetAllAuthorsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}
