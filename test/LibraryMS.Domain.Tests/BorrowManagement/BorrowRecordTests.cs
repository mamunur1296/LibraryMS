using LibraryMS.Domain.BorrowManagement;
using LibraryMS.Domain.Shared.Exceptions;
using FluentAssertions;

namespace LibraryMS.Domain.Tests.BorrowManagement;

/// <summary>
/// Unit tests for BorrowRecord aggregate business rules.
/// Tests are designed to document and enforce domain behavior.
/// </summary>
public class BorrowRecordTests
{
    // ────────────────────────────────────────────────────────────
    // Helpers
    // ────────────────────────────────────────────────────────────
    private static BorrowRecord CreateActiveBorrow(int borrowDays = 14)
    {
        return (BorrowRecord)Activator.CreateInstance(typeof(BorrowRecord), true)!;
        // We use the internal constructor via reflection in tests
    }

    private static BorrowRecord CreateBorrowViaConstructor(int borrowDays = 14)
    {
        // Access internal constructor for testing
        var ctor = typeof(BorrowRecord).GetConstructors(
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .First(c => c.GetParameters().Length == 6);

        return (BorrowRecord)ctor.Invoke([
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), borrowDays
        ]);
    }

    // ────────────────────────────────────────────────────────────
    // Creation Tests
    // ────────────────────────────────────────────────────────────

    [Fact]
    public void Constructor_ShouldSetCorrectDueDate_WhenBorrowDaysProvided()
    {
        // Arrange & Act
        var borrow = CreateBorrowViaConstructor(borrowDays: 14);

        // Assert
        borrow.DueDate.Should().BeCloseTo(DateTime.UtcNow.AddDays(14), precision: TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Constructor_ShouldSetStatusToActive()
    {
        var borrow = CreateBorrowViaConstructor();
        borrow.Status.Should().Be(Shared.Enums.BorrowStatus.Active);
    }

    [Fact]
    public void Constructor_ShouldRaiseDomainEvent()
    {
        var borrow = CreateBorrowViaConstructor();
        borrow.DomainEvents.Should().ContainSingle(e => e is BorrowManagement.Events.BookBorrowedEvent);
    }

    // ────────────────────────────────────────────────────────────
    // Return Tests
    // ────────────────────────────────────────────────────────────

    [Fact]
    public void Return_WhenBookNotYetDue_ShouldSetStatusReturnedAndZeroFine()
    {
        // Arrange
        var borrow = CreateBorrowViaConstructor(borrowDays: 14);

        // Act
        var returnMethod = typeof(BorrowRecord).GetMethod("Return",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        returnMethod!.Invoke(borrow, [null]);

        // Assert
        borrow.Status.Should().Be(Shared.Enums.BorrowStatus.Returned);
        borrow.LateFine.Should().Be(0m);
        borrow.ReturnDate.Should().NotBeNull();
    }

    [Fact]
    public void Return_WhenAlreadyReturned_ShouldThrowDomainException()
    {
        // Arrange
        var borrow = CreateBorrowViaConstructor();
        var returnMethod = typeof(BorrowRecord).GetMethod("Return",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        returnMethod!.Invoke(borrow, [null]); // first return

        // Act
        var act = () => returnMethod.Invoke(borrow, [null]);

        // Assert
        act.Should().Throw<System.Reflection.TargetInvocationException>()
            .WithInnerException<DomainException>()
            .WithMessage("*already been returned*");
    }

    [Fact]
    public void Return_ShouldRaiseBookReturnedEvent()
    {
        var borrow = CreateBorrowViaConstructor();
        borrow.ClearDomainEvents();

        var returnMethod = typeof(BorrowRecord).GetMethod("Return",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        returnMethod!.Invoke(borrow, [null]);

        borrow.DomainEvents.Should().ContainSingle(e => e is BorrowManagement.Events.BookReturnedEvent);
    }

    // ────────────────────────────────────────────────────────────
    // Business Constants Tests
    // ────────────────────────────────────────────────────────────

    [Fact]
    public void MaxBorrowDays_ShouldBe14()
        => BorrowRecord.MaxBorrowDays.Should().Be(14);

    [Fact]
    public void LateFinePerDay_ShouldBe2()
        => BorrowRecord.LateFinePerDay.Should().Be(2.0m);

    [Fact]
    public void MaxActiveBorrowsPerMember_ShouldBe5()
        => BorrowRecord.MaxActiveBorrowsPerMember.Should().Be(5);

    // ────────────────────────────────────────────────────────────
    // IsOverdue Computed Property
    // ────────────────────────────────────────────────────────────

    [Fact]
    public void IsOverdue_WhenWithinDueDate_ShouldBeFalse()
    {
        var borrow = CreateBorrowViaConstructor(borrowDays: 14);
        borrow.IsOverdue.Should().BeFalse();
    }
}
