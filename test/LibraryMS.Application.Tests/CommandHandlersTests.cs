using LibraryMS.Application.Borrows;
using LibraryMS.Application.Contracts.Borrows;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.BookManagement.AggregateRoots;
using LibraryMS.Domain.BookManagement.Entities;
using LibraryMS.Domain.BookManagement.Services;
using LibraryMS.Domain.BorrowManagement;
using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.Shared;
using LibraryMS.TestBase;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LibraryMS.Application.Tests;

public class CommandHandlersTests
{
    private readonly Mock<IBorrowRepository> _borrowRepoMock;
    private readonly Mock<IMemberRepository> _memberRepoMock;
    private readonly Mock<IBookRepository> _bookRepoMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<ILogger<BorrowBookCommandHandler>> _borrowLoggerMock;
    private readonly Mock<ILogger<ReturnBookCommandHandler>> _returnLoggerMock;

    private readonly BorrowManager _borrowManager;
    private readonly BorrowBookCommandHandler _borrowHandler;
    private readonly ReturnBookCommandHandler _returnHandler;

    public CommandHandlersTests()
    {
        _borrowRepoMock = new Mock<IBorrowRepository>();
        _memberRepoMock = new Mock<IMemberRepository>();
        _bookRepoMock = new Mock<IBookRepository>();
        _uowMock = new Mock<IUnitOfWork>();
        _borrowLoggerMock = new Mock<ILogger<BorrowBookCommandHandler>>();
        _returnLoggerMock = new Mock<ILogger<ReturnBookCommandHandler>>();

        _borrowManager = new BorrowManager(
            _borrowRepoMock.Object,
            _memberRepoMock.Object,
            _bookRepoMock.Object);

        _borrowHandler = new BorrowBookCommandHandler(
            _borrowManager,
            _uowMock.Object,
            _borrowLoggerMock.Object);

        _returnHandler = new ReturnBookCommandHandler(
            _borrowManager,
            _uowMock.Object,
            _returnLoggerMock.Object);
    }

    [Fact]
    public async Task BorrowBookCommandHandler_ShouldCallManagerAndSaveChanges()
    {
        // Arrange
        var branch = TestDataFactory.CreateBranch();
        var book = TestDataFactory.CreateBook();
        var copy = book.AddCopy(branch.Id);
        var member = TestDataFactory.CreateMember();

        var command = new BorrowBookCommand(member.Id, copy.Id, book.Id, branch.Id, 14);

        _memberRepoMock.Setup(r => r.GetByIdAsync(member.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(member);
        _memberRepoMock.Setup(r => r.GetActiveBorrowCountAsync(member.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        _borrowRepoMock.Setup(r => r.HasActiveBorrowForCopyAsync(copy.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _bookRepoMock.Setup(r => r.GetByIdWithCopiesAsync(book.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        // Act
        var result = await _borrowHandler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.MemberId.Should().Be(member.Id);
        result.BookId.Should().Be(book.Id);
        result.BranchId.Should().Be(branch.Id);

        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ReturnBookCommandHandler_ShouldUpdateBorrowRecordAndSaveChanges()
    {
        // Arrange
        var branch = TestDataFactory.CreateBranch();
        var book = TestDataFactory.CreateBook();
        var copy = book.AddCopy(branch.Id);
        var member = TestDataFactory.CreateMember();
        var borrow = TestDataFactory.CreateBorrowRecord(copy, member, DateTime.UtcNow);

        var command = new ReturnBookCommand(borrow.Id, "Returned in perfect condition");

        _borrowRepoMock.Setup(r => r.GetByIdAsync(borrow.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(borrow);
        _bookRepoMock.Setup(r => r.GetByIdWithCopiesAsync(borrow.BookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        // Act
        var result = await _returnHandler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be("Returned");

        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}

