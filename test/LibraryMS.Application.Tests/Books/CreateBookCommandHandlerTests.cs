using FluentAssertions;
using LibraryMS.Application.Books;
using LibraryMS.Application.Contracts.Books;
using LibraryMS.Application.Contracts.DTOs.Book;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.BookManagement.AggregateRoots;
using LibraryMS.Domain.BookManagement.Entities;
using LibraryMS.Domain.BookManagement.Services;
using LibraryMS.Domain.BranchManagement;
using LibraryMS.Domain.BranchManagement.AggregateRoots;
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

public class CreateBookCommandHandlerTests
{
    private readonly Mock<IBookRepository> _bookRepoMock;
    private readonly Mock<IGuidGenerator> _guidGenMock;
    private readonly Mock<IBranchRepository> _branchRepoMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<ILogger<CreateBookCommandHandler>> _loggerMock;
    
    private readonly BookManager _bookManager;
    private readonly BookCopyManager _copyManager;
    private readonly CreateBookCommandHandler _handler;

    public CreateBookCommandHandlerTests()
    {
        _bookRepoMock = new Mock<IBookRepository>();
        _guidGenMock = new Mock<IGuidGenerator>();
        _branchRepoMock = new Mock<IBranchRepository>();
        _uowMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<CreateBookCommandHandler>>();

        _bookManager = new BookManager(
            _bookRepoMock.Object,
            _guidGenMock.Object);
            
        _copyManager = new BookCopyManager(_guidGenMock.Object);

        _handler = new CreateBookCommandHandler(
            _bookManager,
            _copyManager,
            _bookRepoMock.Object,
            _uowMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesBookWithCopies()
    {
        // Arrange
        var command = new CreateBookCommand(
            "New Book", "1234567890123", "Desc", 2024, Guid.NewGuid(), Guid.NewGuid(), "EN", 2, Guid.NewGuid());

        _bookRepoMock.Setup(x => x.IsbnExistsAsync("1234567890123", It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("New Book");
        
        _bookRepoMock.Verify(x => x.AddAsync(It.Is<Book>(b => b.Copies.Count == 2), It.IsAny<CancellationToken>()), Times.Once);
        _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DuplicateISBN_ThrowsDomainException()
    {
        var command = new CreateBookCommand(
            "New Book", "1234567890123", "Desc", 2024, Guid.NewGuid(), Guid.NewGuid(), "EN", 2, Guid.NewGuid());

        _bookRepoMock.Setup(x => x.IsbnExistsAsync("1234567890123", It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<DomainException>()
            .WithMessage("*A book with ISBN*already exists*");
            
        _bookRepoMock.Verify(x => x.AddAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
