using FluentAssertions;
using LibraryMS.Application.Books;
using LibraryMS.Application.Contracts.Books;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.BookManagement.AggregateRoots;
using LibraryMS.Domain.BookManagement.Services;
using LibraryMS.Domain.Common;
using LibraryMS.Domain.Shared;
using LibraryMS.Domain.Shared.Exceptions;
using LibraryMS.TestBase;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LibraryMS.Application.Tests.Books;

public class UpdateBookCommandHandlerTests
{
    private readonly Mock<IBookRepository> _bookRepoMock;
    private readonly Mock<IGuidGenerator> _guidGenMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<ILogger<UpdateBookCommandHandler>> _loggerMock;
    
    private readonly BookManager _bookManager;
    private readonly UpdateBookCommandHandler _handler;

    public UpdateBookCommandHandlerTests()
    {
        _bookRepoMock = new Mock<IBookRepository>();
        _guidGenMock = new Mock<IGuidGenerator>();
        _uowMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<UpdateBookCommandHandler>>();

        _bookManager = new BookManager(
            _bookRepoMock.Object,
            _guidGenMock.Object);

        _handler = new UpdateBookCommandHandler(
            _bookManager,
            _bookRepoMock.Object,
            _uowMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_UpdatesBook()
    {
        // Arrange
        var book = TestDataFactory.CreateBook();
        var bookId = book.Id;
        
        var command = new UpdateBookCommand(
            bookId, "Updated Title", "New Desc", 2025, Guid.NewGuid(), Guid.NewGuid(), "EN");

        _bookRepoMock.Setup(x => x.GetByIdWithCopiesAsync(bookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);
            
        // Assuming ISBN isn't changed or isn't duplicate
        _bookRepoMock.Setup(x => x.IsbnExistsAsync(book.ISBN.Value, It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("Updated Title");
        
        _bookRepoMock.Verify(x => x.UpdateAsync(It.Is<Book>(b => b.Title == "Updated Title"), It.IsAny<CancellationToken>()), Times.Once);
        _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_BookNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var command = new UpdateBookCommand(
            bookId, "Updated Title", "New Desc", 2025, Guid.NewGuid(), Guid.NewGuid(), "EN");

        _bookRepoMock.Setup(x => x.GetByIdWithCopiesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Book?)null);

        // Act & Assert
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<NotFoundException>();
    }
}
