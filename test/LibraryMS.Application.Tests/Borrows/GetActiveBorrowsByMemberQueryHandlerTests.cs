using FluentAssertions;
using LibraryMS.Application.Borrows;
using LibraryMS.Application.Contracts.Borrows;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.BorrowManagement;
using LibraryMS.Domain.BorrowManagement.AggregateRoots;
using LibraryMS.Domain.BranchManagement;
using LibraryMS.Domain.MemberManagement;
using LibraryMS.TestBase;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LibraryMS.Application.Tests.Borrows;

public class GetActiveBorrowsByMemberQueryHandlerTests
{
    private readonly Mock<IBorrowRepository> _borrowRepoMock;
    private readonly Mock<IMemberRepository> _memberRepoMock;
    private readonly Mock<IBookRepository> _bookRepoMock;
    private readonly Mock<IBranchRepository> _branchRepoMock;
    private readonly Mock<ILogger<GetActiveBorrowsByMemberQueryHandler>> _loggerMock;
    
    private readonly GetActiveBorrowsByMemberQueryHandler _handler;

    public GetActiveBorrowsByMemberQueryHandlerTests()
    {
        _borrowRepoMock = new Mock<IBorrowRepository>();
        _memberRepoMock = new Mock<IMemberRepository>();
        _bookRepoMock = new Mock<IBookRepository>();
        _branchRepoMock = new Mock<IBranchRepository>();
        _loggerMock = new Mock<ILogger<GetActiveBorrowsByMemberQueryHandler>>();

        _handler = new GetActiveBorrowsByMemberQueryHandler(
            _borrowRepoMock.Object,
            _memberRepoMock.Object,
            _bookRepoMock.Object,
            _branchRepoMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsActiveBorrows()
    {
        // Arrange
        var branch = TestDataFactory.CreateBranch();
        var book = TestDataFactory.CreateBook();
        var copy = book.AddCopy(branch.Id);
        var memberId = Guid.NewGuid();
        var borrow = TestDataFactory.CreateBorrowRecord(copy, TestDataFactory.CreateMember(), DateTime.UtcNow);

        var list = new List<BorrowRecord> { borrow };

        _borrowRepoMock.Setup(r => r.GetActiveBorrowsByMemberAsync(memberId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(list);
            
        _bookRepoMock.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<LibraryMS.Domain.BookManagement.AggregateRoots.Book> { book });
        _branchRepoMock.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<LibraryMS.Domain.BranchManagement.AggregateRoots.Branch> { branch });

        var query = new GetActiveBorrowsByMemberQuery(memberId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result[0].Id.Should().Be(borrow.Id);
        result[0].BookTitle.Should().Be(book.Title);
    }

    [Fact]
    public async Task Handle_EmptyRepository_ReturnsEmptyList()
    {
        // Arrange
        var memberId = Guid.NewGuid();
        _borrowRepoMock.Setup(r => r.GetActiveBorrowsByMemberAsync(memberId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BorrowRecord>());
            
        _memberRepoMock.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<LibraryMS.Domain.MemberManagement.AggregateRoots.Member>());
        _bookRepoMock.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<LibraryMS.Domain.BookManagement.AggregateRoots.Book>());
        _branchRepoMock.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<LibraryMS.Domain.BranchManagement.AggregateRoots.Branch>());

        var query = new GetActiveBorrowsByMemberQuery(memberId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}
