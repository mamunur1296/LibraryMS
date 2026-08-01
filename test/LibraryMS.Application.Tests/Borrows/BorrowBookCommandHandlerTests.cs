using FluentAssertions;
using LibraryMS.Application.Borrows;
using LibraryMS.Application.Contracts.Borrows;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.BookManagement.AggregateRoots;
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

public class BorrowBookCommandHandlerTests
{
    private readonly Mock<IBorrowRepository> _borrowRepoMock;
    private readonly Mock<IMemberRepository> _memberRepoMock;
    private readonly Mock<IBookRepository> _bookRepoMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<ILogger<BorrowBookCommandHandler>> _loggerMock;
    
    private readonly BorrowManager _borrowManager;
    private readonly BorrowBookCommandHandler _handler;

    public BorrowBookCommandHandlerTests()
    {
        _borrowRepoMock = new Mock<IBorrowRepository>();
        _memberRepoMock = new Mock<IMemberRepository>();
        _bookRepoMock = new Mock<IBookRepository>();
        _uowMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<BorrowBookCommandHandler>>();

        _borrowManager = new BorrowManager(
            _borrowRepoMock.Object,
            _memberRepoMock.Object,
            _bookRepoMock.Object);

        _handler = new BorrowBookCommandHandler(
            _borrowManager,
            _uowMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidBorrow_ReturnsBorrowDto()
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
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.MemberId.Should().Be(member.Id);
        result.BookId.Should().Be(book.Id);
        
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _borrowRepoMock.Verify(r => r.AddAsync(It.IsAny<LibraryMS.Domain.BorrowManagement.AggregateRoots.BorrowRecord>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_MemberNotFound_ThrowsDomainException()
    {
        // Arrange
        var branch = TestDataFactory.CreateBranch();
        var book = TestDataFactory.CreateBook();
        var copy = book.AddCopy(branch.Id);
        var memberId = Guid.NewGuid();

        var command = new BorrowBookCommand(memberId, copy.Id, book.Id, branch.Id, 14);

        _memberRepoMock.Setup(r => r.GetByIdAsync(memberId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((LibraryMS.Domain.MemberManagement.AggregateRoots.Member?)null);

        // Act & Assert
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        
        await action.Should().ThrowAsync<NotFoundException>();
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_BookCopyNotAvailable_ThrowsDomainException()
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
            .ReturnsAsync(true); // Copy is already borrowed
            
        _bookRepoMock.Setup(r => r.GetByIdWithCopiesAsync(book.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        // Act & Assert
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        
        await action.Should().ThrowAsync<DomainException>()
            .WithMessage("*already borrowed*");
            
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
