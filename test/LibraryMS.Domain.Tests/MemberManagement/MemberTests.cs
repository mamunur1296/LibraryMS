using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.Shared.Exceptions;
using LibraryMS.Domain.Shared.Enums;
using FluentAssertions;
using System;
using Xunit;

namespace LibraryMS.Domain.Tests.MemberManagement;

public class MemberTests
{
    private static Member CreateMember(string email = "test@example.com")
    {
        return new Member(
            Guid.NewGuid(),
            "John",
            "Doe",
            email,
            "+8801711111111",
            "LIB-2024-00001",
            "123 Main St");
    }

    [Fact]
    public void Constructor_ShouldSetStatusActive()
    {
        var member = CreateMember();
        member.Status.Should().Be(MemberStatus.Active);
    }

    [Fact]
    public void Constructor_ShouldRaiseMemberRegisteredEvent()
    {
        var member = CreateMember();
        member.DomainEvents.Should().ContainSingle(e => e is LibraryMS.Domain.MemberManagement.Events.MemberRegisteredEvent);
    }

    [Fact]
    public void FullName_ShouldCombineFirstAndLastName()
    {
        var member = CreateMember();
        member.FullName.Should().Be("John Doe");
    }

    [Fact]
    public void CanBorrow_WhenActive_ShouldReturnTrue()
    {
        var member = CreateMember();
        member.CanBorrow().Should().BeTrue();
    }

    [Fact]
    public void Suspend_WhenActive_ShouldChangeStatus()
    {
        var member = CreateMember();
        member.Suspend(DateTime.UtcNow.AddDays(7), "Overdue books");

        member.Status.Should().Be(MemberStatus.Suspended);
        member.CanBorrow().Should().Be(false);
    }

    [Fact]
    public void Suspend_WhenAlreadySuspended_ShouldThrowDomainException()
    {
        var member = CreateMember();
        member.Suspend(DateTime.UtcNow.AddDays(7), "First suspension");
        
        var act = () => member.Suspend(DateTime.UtcNow.AddDays(7), "Second suspension");

        act.Should().Throw<DomainException>()
            .WithMessage("*already suspended*");
    }

    [Fact]
    public void Activate_WhenSuspended_ShouldRestoreActiveStatus()
    {
        var member = CreateMember();
        member.Suspend(DateTime.UtcNow.AddDays(7), "Reason");
        member.Activate();

        member.Status.Should().Be(MemberStatus.Active);
        member.CanBorrow().Should().BeTrue();
    }

    [Fact]
    public void CanBorrow_WhenSuspensionExpired_ShouldAutoLiftSuspensionAndReturnTrue()
    {
        // Arrange
        var member = CreateMember();
        
        // Suspend until 1 hour ago
        member.Suspend(DateTime.UtcNow.AddHours(-1), "Expired suspension");

        // Act
        var result = member.CanBorrow();

        // Assert
        result.Should().BeTrue();
        member.Status.Should().Be(MemberStatus.Active);
        member.SuspendedUntil.Should().BeNull();
    }
}
