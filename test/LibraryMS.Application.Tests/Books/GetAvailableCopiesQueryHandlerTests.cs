using FluentAssertions;
using LibraryMS.Application.Books;
using LibraryMS.Application.Contracts.Books;
using LibraryMS.Application.Contracts.DTOs.Book;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.BookManagement.Entities;
using LibraryMS.TestBase;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LibraryMS.Application.Tests.Books;

public class GetAvailableCopiesQueryHandlerTests
{
    private readonly Mock<IBookRepository> _bookRepoMock;
    private readonly GetAvailableCopiesQueryHandler _handler;

    public GetAvailableCopiesQueryHandlerTests()
    {
        _bookRepoMock = new Mock<IBookRepository>();
        _handler = new GetAvailableCopiesQueryHandler(_bookRepoMock.Object);
    }

    [Fact]
    public async Task Handle_BookNotFound_ReturnsEmptyList()
    {
        // Arrange
        _bookRepoMock.Setup(r => r.GetByIdWithCopiesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((LibraryMS.Domain.BookManagement.AggregateRoots.Book?)null);
            
        var query = new GetAvailableCopiesQuery(Guid.NewGuid(), null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_BookFound_ReturnsAvailableCopies()
    {
        // Arrange
        var branch1 = TestDataFactory.CreateBranch();
        var branch2 = TestDataFactory.CreateBranch();
        var book = TestDataFactory.CreateBook();
        
        book.AddCopy(branch1.Id);
        book.AddCopy(branch2.Id);
        
        _bookRepoMock.Setup(r => r.GetByIdWithCopiesAsync(book.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);
            
        var query = new GetAvailableCopiesQuery(book.Id, null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_WithBranchId_ReturnsCopiesFromBranch()
    {
        // Arrange
        var branch1 = TestDataFactory.CreateBranch();
        var branch2 = TestDataFactory.CreateBranch();
        var book = TestDataFactory.CreateBook();
        
        book.AddCopy(branch1.Id);
        book.AddCopy(branch2.Id);
        
        _bookRepoMock.Setup(r => r.GetByIdWithCopiesAsync(book.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);
            
        var query = new GetAvailableCopiesQuery(book.Id, branch1.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result[0].BranchId.Should().Be(branch1.Id);
    }
}
