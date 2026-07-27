using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.Shared.Exceptions;
using FluentAssertions;

namespace LibraryMS.Domain.Tests.MemberManagement;

public class MemberTests
{
    private static Member CreateMember(string email = "test@example.com")
    {
        var ctor = typeof(Member).GetConstructors(
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .First(c => c.GetParameters().Length == 7);

        return (Member)ctor.Invoke([
            Guid.NewGuid(), "John", "Doe", email, "+8801711111111", "LIB-2024-00001", null
        ]);
    }

    [Fact]
    public void Constructor_ShouldSetStatusActive()
    {
        var member = CreateMember();
        member.Status.Should().Be(Shared.Enums.MemberStatus.Active);
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
    public void Suspend_WhenActive_ShouldChangStatus()
    {
        var member = CreateMember();
        var suspendMethod = typeof(Member).GetMethod("Suspend",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        suspendMethod!.Invoke(member, [DateTime.UtcNow.AddDays(7), "Overdue books"]);

        member.Status.Should().Be(Shared.Enums.MemberStatus.Suspended);
        member.CanBorrow().Should().BeFalse();
    }

    [Fact]
    public void Suspend_WhenAlreadySuspended_ShouldThrowDomainException()
    {
        var member = CreateMember();
        var suspendMethod = typeof(Member).GetMethod("Suspend",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        suspendMethod!.Invoke(member, [DateTime.UtcNow.AddDays(7), "First suspension"]);
        var act = () => suspendMethod.Invoke(member, [DateTime.UtcNow.AddDays(7), "Second suspension"]);

        act.Should().Throw<System.Reflection.TargetInvocationException>()
            .WithInnerException<DomainException>()
            .WithMessage("*already suspended*");
    }

    [Fact]
    public void Activate_WhenSuspended_ShouldRestoreActiveStatus()
    {
        var member = CreateMember();
        var suspendMethod = typeof(Member).GetMethod("Suspend",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var activateMethod = typeof(Member).GetMethod("Activate",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        suspendMethod!.Invoke(member, [DateTime.UtcNow.AddDays(7), "Reason"]);
        activateMethod!.Invoke(member, []);

        member.Status.Should().Be(Shared.Enums.MemberStatus.Active);
        member.CanBorrow().Should().BeTrue();
    }
}
