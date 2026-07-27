using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.ReservationManagement;
using LibraryMS.Domain.Shared.Enums;
using LibraryMS.Domain.Shared.Exceptions;
using LibraryMS.TestBase;
using FluentAssertions;
using System;
using Xunit;

namespace LibraryMS.Domain.Tests.ReservationManagement;

public class ReservationQueueTests
{
    private static Reservation CreateReservation(Guid memberId, Guid bookId, Guid branchId, int queuePosition)
    {
        return new Reservation(
            Guid.NewGuid(),
            memberId,
            bookId,
            branchId,
            queuePosition);
    }

    [Fact]
    public void Constructor_ShouldSetStatusToPendingAndQueuePosition()
    {
        // Arrange
        var memberId = Guid.NewGuid();
        var bookId = Guid.NewGuid();
        var branchId = Guid.NewGuid();

        // Act
        var reservation = CreateReservation(memberId, bookId, branchId, 3);

        // Assert
        reservation.Status.Should().Be(ReservationStatus.Pending);
        reservation.QueuePosition.Should().Be(3);
        reservation.DomainEvents.Should().ContainSingle(e => e is LibraryMS.Domain.ReservationManagement.Events.ReservationCreatedEvent);
    }

    [Fact]
    public void NotifyAvailable_WhenPending_ShouldChangeStatusToAvailableAndSetExpiryDate()
    {
        var reservation = CreateReservation(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 1);

        reservation.NotifyAvailable();

        reservation.Status.Should().Be(ReservationStatus.Available);
        reservation.NotifiedAt.Should().NotBeNull();
        reservation.ExpiresAt.Should().BeCloseTo(DateTime.UtcNow.AddDays(Reservation.ExpiryDaysAfterNotification), precision: TimeSpan.FromSeconds(5));
        reservation.DomainEvents.Should().Contain(e => e is LibraryMS.Domain.ReservationManagement.Events.ReservationAvailableEvent);
    }

    [Fact]
    public void Fulfill_WhenAvailable_ShouldChangeStatusToFulfilled()
    {
        var reservation = CreateReservation(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 1);
        reservation.NotifyAvailable();

        reservation.Fulfill();

        reservation.Status.Should().Be(ReservationStatus.Fulfilled);
        reservation.FulfilledAt.Should().NotBeNull();
    }

    [Fact]
    public void Cancel_WhenPendingOrAvailable_ShouldSucceed()
    {
        var reservation = CreateReservation(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 1);

        reservation.Cancel();

        reservation.Status.Should().Be(ReservationStatus.Cancelled);
        reservation.CancelledAt.Should().NotBeNull();
        reservation.DomainEvents.Should().Contain(e => e is LibraryMS.Domain.ReservationManagement.Events.ReservationCancelledEvent);
    }

    [Fact]
    public void Expire_WhenAvailable_ShouldSetStatusToExpired()
    {
        var reservation = CreateReservation(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 1);
        reservation.NotifyAvailable();

        reservation.Expire();

        reservation.Status.Should().Be(ReservationStatus.Expired);
        reservation.DomainEvents.Should().Contain(e => e is LibraryMS.Domain.ReservationManagement.Events.ReservationExpiredEvent);
    }

    [Fact]
    public void UpdateQueuePosition_WithValidPosition_ShouldSucceed()
    {
        var reservation = CreateReservation(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 3);

        reservation.UpdateQueuePosition(1);

        reservation.QueuePosition.Should().Be(1);
    }

    [Fact]
    public void UpdateQueuePosition_WithInvalidPosition_ShouldThrowDomainException()
    {
        var reservation = CreateReservation(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 3);

        var act = () => reservation.UpdateQueuePosition(0);

        act.Should().Throw<DomainException>()
            .WithMessage("*position must be at least 1*");
    }
}
