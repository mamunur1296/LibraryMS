using FluentAssertions;
using LibraryMS.Application.Contracts.Reservations;
using LibraryMS.Application.Reservations;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.ReservationManagement;
using LibraryMS.Domain.ReservationManagement.AggregateRoots;
using LibraryMS.Domain.Shared;
using LibraryMS.Domain.Shared.Enums;
using LibraryMS.Domain.Shared.Exceptions;
using LibraryMS.TestBase;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LibraryMS.Application.Tests.Reservations;

public class CancelReservationCommandHandlerTests
{
    private readonly Mock<IReservationRepository> _reservationRepoMock;
    private readonly Mock<IMemberRepository> _memberRepoMock;
    private readonly Mock<IBookRepository> _bookRepoMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<ILogger<CancelReservationCommandHandler>> _loggerMock;
    
    private readonly CancelReservationCommandHandler _handler;

    public CancelReservationCommandHandlerTests()
    {
        _reservationRepoMock = new Mock<IReservationRepository>();
        _memberRepoMock = new Mock<IMemberRepository>();
        _bookRepoMock = new Mock<IBookRepository>();
        _uowMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<CancelReservationCommandHandler>>();

        _handler = new CancelReservationCommandHandler(
            _reservationRepoMock.Object,
            _uowMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidReservation_CancelsReservation()
    {
        // Arrange
        var reservationId = Guid.NewGuid();
        var book = TestDataFactory.CreateBook();
        var member = TestDataFactory.CreateMember();
        var reservation = TestDataFactory.CreateReservation(book, member);
        
        _reservationRepoMock.Setup(x => x.GetByIdAsync(reservationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(reservation);
            
        _reservationRepoMock.Setup(x => x.GetQueueForBookAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Reservation>());

        var command = new CancelReservationCommand(reservationId, member.Id);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        reservation.Status.Should().Be(LibraryMS.Domain.Shared.Enums.ReservationStatus.Cancelled);
        
        _reservationRepoMock.Verify(x => x.UpdateAsync(reservation, It.IsAny<CancellationToken>()), Times.Once);
        _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ReservationNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var reservationId = Guid.NewGuid();
        var requestingMemberId = Guid.NewGuid();
        _reservationRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Reservation?)null);

        var command = new CancelReservationCommand(reservationId, requestingMemberId);

        // Act & Assert
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<NotFoundException>();
    }
}
