using FluentAssertions;
using LibraryMS.Application.Borrows;
using LibraryMS.Application.Contracts.Borrows;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.BorrowManagement;
using LibraryMS.Domain.BorrowManagement.Services;
using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.Shared;
using LibraryMS.Domain.Shared.Exceptions;
using LibraryMS.TestBase;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LibraryMS.Application.Tests.Borrows;

public class ReturnBookCommandHandlerTests
{
    private readonly Mock<IBorrowRepository> _borrowRepoMock;
    private readonly Mock<IMemberRepository> _memberRepoMock;
    private readonly Mock<IBookRepository> _bookRepoMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<ILogger<ReturnBookCommandHandler>> _loggerMock;
    
    private readonly BorrowManager _borrowManager;
    private readonly ReturnBookCommandHandler _handler;

    public ReturnBookCommandHandlerTests()
    {
        _borrowRepoMock = new Mock<IBorrowRepository>();
        _memberRepoMock = new Mock<IMemberRepository>();
        _bookRepoMock = new Mock<IBookRepository>();
        _uowMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<ReturnBookCommandHandler>>();

        _borrowManager = new BorrowManager(
            _borrowRepoMock.Object,
            _memberRepoMock.Object,
            _bookRepoMock.Object);

        _handler = new ReturnBookCommandHandler(
            _borrowManager,
            _uowMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidReturn_ReturnsBorrowDto()
    {
        // Arrange
        var branch = TestDataFactory.CreateBranch();
        var book = TestDataFactory.CreateBook();
        var copy = book.AddCopy(branch.Id);
        var member = TestDataFactory.CreateMember();
        var borrow = TestDataFactory.CreateBorrowRecord(copy, member, DateTime.UtcNow.AddDays(-5));

        var command = new ReturnBookCommand(borrow.Id, "Returned in perfect condition");

        _borrowRepoMock.Setup(r => r.GetByIdAsync(borrow.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(borrow);
        _bookRepoMock.Setup(r => r.GetByIdWithCopiesAsync(borrow.BookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be("Returned");
        result.ReturnDate.Should().HaveValue();
        
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _borrowRepoMock.Verify(r => r.UpdateAsync(borrow, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_RecordNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var command = new ReturnBookCommand(Guid.NewGuid(), "Returned in perfect condition");

        _borrowRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((LibraryMS.Domain.BorrowManagement.AggregateRoots.BorrowRecord?)null);

        // Act & Assert
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        
        await action.Should().ThrowAsync<NotFoundException>();
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_BookAlreadyReturned_ThrowsDomainException()
    {
        // Arrange
        var branch = TestDataFactory.CreateBranch();
        var book = TestDataFactory.CreateBook();
        var copy = book.AddCopy(branch.Id);
        var member = TestDataFactory.CreateMember();
        var borrow = TestDataFactory.CreateBorrowRecord(copy, member, DateTime.UtcNow.AddDays(-5));
        
        borrow.Return("Good condition"); // Already returned

        var command = new ReturnBookCommand(borrow.Id, "Trying to return again");

        _borrowRepoMock.Setup(r => r.GetByIdAsync(borrow.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(borrow);
        _bookRepoMock.Setup(r => r.GetByIdWithCopiesAsync(borrow.BookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        // Act & Assert
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        
        await action.Should().ThrowAsync<DomainException>()
            .WithMessage("*already been returned*");
            
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
