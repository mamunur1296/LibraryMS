using FluentAssertions;
using LibraryMS.Application.Contracts.Reservations;
using LibraryMS.Application.Reservations;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.BookManagement.AggregateRoots;
using LibraryMS.Domain.BranchManagement;
using LibraryMS.Domain.BranchManagement.AggregateRoots;
using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.ReservationManagement;
using LibraryMS.Domain.ReservationManagement.AggregateRoots;
using LibraryMS.TestBase;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LibraryMS.Application.Tests.Reservations;

public class GetMemberReservationsQueryHandlerTests
{
    private readonly Mock<IReservationRepository> _reservationRepoMock;
    private readonly Mock<IMemberRepository> _memberRepoMock;
    private readonly Mock<IBookRepository> _bookRepoMock;
    private readonly Mock<IBranchRepository> _branchRepoMock;
    private readonly Mock<ILogger<GetMemberReservationsQueryHandler>> _loggerMock;
    
    private readonly GetMemberReservationsQueryHandler _handler;

    public GetMemberReservationsQueryHandlerTests()
    {
        _reservationRepoMock = new Mock<IReservationRepository>();
        _memberRepoMock = new Mock<IMemberRepository>();
        _bookRepoMock = new Mock<IBookRepository>();
        _branchRepoMock = new Mock<IBranchRepository>();
        _loggerMock = new Mock<ILogger<GetMemberReservationsQueryHandler>>();

        _handler = new GetMemberReservationsQueryHandler(
            _reservationRepoMock.Object,
            _memberRepoMock.Object,
            _bookRepoMock.Object,
            _branchRepoMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidMemberId_ReturnsReservations()
    {
        // Arrange
        var member = TestDataFactory.CreateMember();
        var memberId = member.Id;
        var book = TestDataFactory.CreateBook();
        var reservation = TestDataFactory.CreateReservation(book, member);
        var list = new List<Reservation> { reservation };

        _reservationRepoMock.Setup(x => x.GetPagedAsync(memberId, null, null, 1, int.MaxValue, It.IsAny<CancellationToken>()))
            .ReturnsAsync((list, 1));
            
        _memberRepoMock.Setup(x => x.GetByIdAsync(memberId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(member);
        _bookRepoMock.Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Book> { book });
        var branch = TestDataFactory.CreateBranch();
        _branchRepoMock.Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Branch> { branch });

        var query = new GetMemberReservationsQuery(memberId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_EmptyRepository_ReturnsEmptyList()
    {
        // Arrange
        var memberId = Guid.NewGuid();
        _reservationRepoMock.Setup(x => x.GetPagedAsync(memberId, null, null, 1, int.MaxValue, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Reservation>(), 0));
            
        _memberRepoMock.Setup(x => x.GetByIdAsync(memberId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestDataFactory.CreateMember());

        _bookRepoMock.Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Book>());
        _branchRepoMock.Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Branch>());

        var query = new GetMemberReservationsQuery(memberId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}
