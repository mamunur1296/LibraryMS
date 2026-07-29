using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.BookManagement.AggregateRoots;
using LibraryMS.Domain.BookManagement.Entities;
using LibraryMS.Domain.BookManagement.Services;
using LibraryMS.Domain.BorrowManagement;
using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.Shared.Exceptions;
using LibraryMS.Domain.Shared.Enums;
using LibraryMS.TestBase;
using FluentAssertions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LibraryMS.Domain.Tests.BorrowManagement;

public class BorrowManagerTests
{
    private readonly Mock<IBorrowRepository> _borrowRepoMock;
    private readonly Mock<IMemberRepository> _memberRepoMock;
    private readonly Mock<IBookRepository> _bookRepoMock;
    private readonly BorrowManager _borrowManager;

    public BorrowManagerTests()
    {
        _borrowRepoMock = new Mock<IBorrowRepository>();
        _memberRepoMock = new Mock<IMemberRepository>();
        _bookRepoMock = new Mock<IBookRepository>();
        
        _borrowManager = new BorrowManager(
            _borrowRepoMock.Object,
            _memberRepoMock.Object,
            _bookRepoMock.Object);
    }

    [Fact]
    public async Task BorrowAsync_WithValidInputs_ShouldSucceedAndReturnBorrowRecord()
    {
        // Arrange
        var branch = TestDataFactory.CreateBranch();
        var book = TestDataFactory.CreateBook();
        var copy = book.AddCopy(branch.Id);
        var member = TestDataFactory.CreateMember();

        _memberRepoMock.Setup(r => r.GetByIdAsync(member.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(member);
        _memberRepoMock.Setup(r => r.GetActiveBorrowCountAsync(member.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        _borrowRepoMock.Setup(r => r.HasActiveBorrowForCopyAsync(copy.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _bookRepoMock.Setup(r => r.GetByIdWithCopiesAsync(book.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        // Act
        var result = await _borrowManager.BorrowAsync(member.Id, copy.Id, book.Id, branch.Id);

        // Assert
        result.Should().NotBeNull();
        result.MemberId.Should().Be(member.Id);
        result.BookCopyId.Should().Be(copy.Id);
        result.Status.Should().Be(BorrowStatus.Active);
        
        _bookRepoMock.Verify(r => r.UpdateAsync(book, It.IsAny<CancellationToken>()), Times.Once);
        _borrowRepoMock.Verify(r => r.AddAsync(It.IsAny<BorrowRecord>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task BorrowAsync_WhenMemberSuspended_ShouldThrowDomainException()
    {
        // Arrange
        var branch = TestDataFactory.CreateBranch();
        var book = TestDataFactory.CreateBook();
        var copy = book.AddCopy(branch.Id);
        var member = TestDataFactory.CreateMember(status: MemberStatus.Suspended);

        _memberRepoMock.Setup(r => r.GetByIdAsync(member.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(member);

        // Act
        var act = () => _borrowManager.BorrowAsync(member.Id, copy.Id, book.Id, branch.Id);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*suspended and cannot borrow*");
    }

    [Fact]
    public async Task BorrowAsync_WhenMaxLimitReached_ShouldThrowDomainException()
    {
        // Arrange
        var branch = TestDataFactory.CreateBranch();
        var book = TestDataFactory.CreateBook();
        var copy = book.AddCopy(branch.Id);
        var member = TestDataFactory.CreateMember();

        _memberRepoMock.Setup(r => r.GetByIdAsync(member.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(member);
        _memberRepoMock.Setup(r => r.GetActiveBorrowCountAsync(member.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BorrowRecord.MaxActiveBorrowsPerMember); // Limit reached

        // Act
        var act = () => _borrowManager.BorrowAsync(member.Id, copy.Id, book.Id, branch.Id);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*maximum of 5 active borrows*");
    }
}

