using LibraryMS.Domain.BorrowManagement;
using LibraryMS.Domain.Shared.Exceptions;
using LibraryMS.Domain.Shared.Enums;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace LibraryMS.Domain.Tests.BorrowManagement;

/// <summary>
/// Unit tests for BorrowRecord aggregate business rules.
/// Tests are designed to document and enforce domain behavior.
/// </summary>
public class BorrowRecordTests
{
    private static BorrowRecord CreateBorrowRecord(int borrowDays = 14)
    {
        return new BorrowRecord(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            borrowDays);
    }

    [Fact]
    public void Constructor_ShouldSetCorrectDueDate_WhenBorrowDaysProvided()
    {
        var borrow = CreateBorrowRecord(borrowDays: 14);

        borrow.DueDate.Should().BeCloseTo(DateTime.UtcNow.AddDays(14), precision: TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Constructor_ShouldSetStatusToActive()
    {
        var borrow = CreateBorrowRecord();
        borrow.Status.Should().Be(BorrowStatus.Active);
    }

    [Fact]
    public void Constructor_ShouldRaiseDomainEvent()
    {
        var borrow = CreateBorrowRecord();
        borrow.DomainEvents.Should().ContainSingle(e => e is LibraryMS.Domain.BorrowManagement.Events.BookBorrowedEvent);
    }

    [Fact]
    public void Return_WhenBookNotYetDue_ShouldSetStatusReturnedAndZeroFine()
    {
        var borrow = CreateBorrowRecord(borrowDays: 14);

        borrow.Return();

        borrow.Status.Should().Be(BorrowStatus.Returned);
        borrow.LateFine.Should().Be(0m);
        borrow.ReturnDate.Should().NotBeNull();
    }

    [Fact]
    public void Return_WhenAlreadyReturned_ShouldThrowDomainException()
    {
        var borrow = CreateBorrowRecord();
        borrow.Return(); // first return

        var act = () => borrow.Return();

        act.Should().Throw<DomainException>()
            .WithMessage("*already been returned*");
    }

    [Fact]
    public void Return_ShouldRaiseBookReturnedEvent()
    {
        var borrow = CreateBorrowRecord();
        borrow.ClearDomainEvents();

        borrow.Return();

        borrow.DomainEvents.Should().ContainSingle(e => e is LibraryMS.Domain.BorrowManagement.Events.BookReturnedEvent);
    }

    [Fact]
    public void MaxBorrowDays_ShouldBe14()
        => BorrowRecord.MaxBorrowDays.Should().Be(14);

    [Fact]
    public void LateFinePerDay_ShouldBe2()
        => BorrowRecord.LateFinePerDay.Should().Be(2.0m);

    [Fact]
    public void MaxActiveBorrowsPerMember_ShouldBe5()
        => BorrowRecord.MaxActiveBorrowsPerMember.Should().Be(5);

    [Fact]
    public void IsOverdue_WhenWithinDueDate_ShouldBeFalse()
    {
        var borrow = CreateBorrowRecord(borrowDays: 14);
        borrow.IsOverdue.Should().BeFalse();
    }
}
