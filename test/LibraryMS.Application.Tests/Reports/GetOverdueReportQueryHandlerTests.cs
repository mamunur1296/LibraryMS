using FluentAssertions;
using LibraryMS.Application.Contracts.Common;
using LibraryMS.Application.Contracts.Reports;
using LibraryMS.Application.Reports;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.BookManagement.AggregateRoots;
using LibraryMS.Domain.BorrowManagement;
using LibraryMS.Domain.BorrowManagement.AggregateRoots;
using LibraryMS.Domain.BranchManagement;
using LibraryMS.Domain.BranchManagement.AggregateRoots;
using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.MemberManagement.AggregateRoots;
using LibraryMS.TestBase;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LibraryMS.Application.Tests.Reports;

public class GetOverdueReportQueryHandlerTests
{
    private readonly Mock<IBorrowRepository> _borrowRepoMock;
    private readonly Mock<IBookRepository> _bookRepoMock;
    private readonly Mock<IMemberRepository> _memberRepoMock;
    private readonly Mock<IBranchRepository> _branchRepoMock;
    private readonly Mock<ILogger<GetOverdueReportQueryHandler>> _loggerMock;
    
    private readonly GetOverdueReportQueryHandler _handler;

    public GetOverdueReportQueryHandlerTests()
    {
        _borrowRepoMock = new Mock<IBorrowRepository>();
        _bookRepoMock = new Mock<IBookRepository>();
        _memberRepoMock = new Mock<IMemberRepository>();
        _branchRepoMock = new Mock<IBranchRepository>();
        _loggerMock = new Mock<ILogger<GetOverdueReportQueryHandler>>();

        _handler = new GetOverdueReportQueryHandler(
            _borrowRepoMock.Object,
            _bookRepoMock.Object,
            _memberRepoMock.Object,
            _branchRepoMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsOverdueRecords()
    {
        // Arrange
        var branch = TestDataFactory.CreateBranch();
        var book = TestDataFactory.CreateBook();
        var member = TestDataFactory.CreateMember();
        var bookCopy = TestDataFactory.CreateBookCopy(book, branch);
        
        var borrow = TestDataFactory.CreateBorrowRecord(bookCopy, member, DateTime.UtcNow.AddDays(-20)); // Overdue
        
        var borrowList = new List<BorrowRecord> { borrow };

        _borrowRepoMock.Setup(x => x.GetPagedAsync(null, null, "Overdue", 1, 10, It.IsAny<CancellationToken>(), null, null, null))
            .ReturnsAsync((borrowList, 1));
            
        _memberRepoMock.Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Member>());
        _bookRepoMock.Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Book>());
        _branchRepoMock.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Branch>());

        var query = new GetOverdueReportQuery(null, null, null, 1, 10);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TotalCount.Should().Be(1);
        result.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_EmptyRepository_ReturnsEmptyPagedResult()
    {
        // Arrange
        _borrowRepoMock.Setup(x => x.GetPagedAsync(null, null, "Overdue", 1, 10, It.IsAny<CancellationToken>(), null, null, null))
            .ReturnsAsync((new List<BorrowRecord>(), 0));
        _memberRepoMock.Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Member>());
        _bookRepoMock.Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Book>());
        _branchRepoMock.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Branch>());

        var query = new GetOverdueReportQuery(null, null, null, 1, 10);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TotalCount.Should().Be(0);
        result.Items.Should().BeEmpty();
    }
}
