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

public class GetAllCategoriesQueryHandlerTests
{
    private readonly Mock<IBookRepository> _bookRepoMock;
    private readonly GetAllCategoriesQueryHandler _handler;

    public GetAllCategoriesQueryHandlerTests()
    {
        _bookRepoMock = new Mock<IBookRepository>();
        _handler = new GetAllCategoriesQueryHandler(_bookRepoMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsAllCategories()
    {
        // Arrange
        var categories = new List<Category>
        {
            new Category(Guid.NewGuid(), "Cat 1", "Desc 1"),
            new Category(Guid.NewGuid(), "Cat 2", "Desc 2")
        };
        
        _bookRepoMock.Setup(r => r.GetAllCategoriesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);
            
        var query = new GetAllCategoriesQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result[0].Name.Should().Be("Cat 1");
    }

    [Fact]
    public async Task Handle_NoCategories_ReturnsEmptyList()
    {
        // Arrange
        _bookRepoMock.Setup(r => r.GetAllCategoriesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Category>());
            
        var query = new GetAllCategoriesQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}
