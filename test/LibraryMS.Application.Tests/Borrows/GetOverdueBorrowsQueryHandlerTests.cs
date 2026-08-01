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

public class GetOverdueBorrowsQueryHandlerTests
{
    private readonly Mock<IBorrowRepository> _borrowRepoMock;
    private readonly Mock<IMemberRepository> _memberRepoMock;
    private readonly Mock<IBookRepository> _bookRepoMock;
    private readonly Mock<IBranchRepository> _branchRepoMock;
    private readonly Mock<ILogger<GetOverdueBorrowsQueryHandler>> _loggerMock;
    
    private readonly GetOverdueBorrowsQueryHandler _handler;

    public GetOverdueBorrowsQueryHandlerTests()
    {
        _borrowRepoMock = new Mock<IBorrowRepository>();
        _memberRepoMock = new Mock<IMemberRepository>();
        _bookRepoMock = new Mock<IBookRepository>();
        _branchRepoMock = new Mock<IBranchRepository>();
        _loggerMock = new Mock<ILogger<GetOverdueBorrowsQueryHandler>>();

        _handler = new GetOverdueBorrowsQueryHandler(
            _borrowRepoMock.Object,
            _memberRepoMock.Object,
            _bookRepoMock.Object,
            _branchRepoMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsOverdueBorrows()
    {
        // Arrange
        var branch = TestDataFactory.CreateBranch();
        var book = TestDataFactory.CreateBook();
        var copy = book.AddCopy(branch.Id);
        var member = TestDataFactory.CreateMember();
        var borrow = TestDataFactory.CreateBorrowRecord(copy, member, DateTime.UtcNow.AddDays(-20));

        var list = new List<BorrowRecord> { borrow };

        _borrowRepoMock.Setup(r => r.GetPagedAsync(null, null, "Overdue", 1, 10, It.IsAny<CancellationToken>(), null, null, null))
            .ReturnsAsync((list, 1));
            
        _memberRepoMock.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<LibraryMS.Domain.MemberManagement.AggregateRoots.Member> { member });
        _bookRepoMock.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<LibraryMS.Domain.BookManagement.AggregateRoots.Book> { book });
        _branchRepoMock.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<LibraryMS.Domain.BranchManagement.AggregateRoots.Branch> { branch });

        var query = new GetOverdueBorrowsQuery(1, 10);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TotalCount.Should().Be(1);
        result.Items.Should().HaveCount(1);
        result.Items[0].Id.Should().Be(borrow.Id);
    }
}
