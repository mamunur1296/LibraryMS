using FluentAssertions;
using LibraryMS.Application.Contracts.Members;
using LibraryMS.Application.Members;
using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.MemberManagement.AggregateRoots;
using LibraryMS.Domain.MemberManagement.Services;
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

namespace LibraryMS.Application.Tests.Members;

public class SuspendMemberCommandHandlerTests
{
    private readonly Mock<IMemberRepository> _memberRepoMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<ILogger<SuspendMemberCommandHandler>> _loggerMock;
    
    private readonly MemberManager _memberManager;
    private readonly SuspendMemberCommandHandler _handler;

    public SuspendMemberCommandHandlerTests()
    {
        _memberRepoMock = new Mock<IMemberRepository>();
        _uowMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<SuspendMemberCommandHandler>>();
        
        _memberManager = new MemberManager(_memberRepoMock.Object);

        _handler = new SuspendMemberCommandHandler(
            _memberManager,
            _memberRepoMock.Object,
            _uowMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidMember_SuspendsMember()
    {
        // Arrange
        var memberId = Guid.NewGuid();
        var member = TestDataFactory.CreateMember();
        
        _memberRepoMock.Setup(x => x.GetByIdAsync(memberId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(member);

        var suspendUntil = DateTime.UtcNow.AddDays(7);
        var command = new SuspendMemberCommand(memberId, suspendUntil, "Violation of rules");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        member.Status.Should().Be(MemberStatus.Suspended);
        member.SuspendedUntil.Should().Be(suspendUntil);
        
        _memberRepoMock.Verify(x => x.UpdateAsync(member, It.IsAny<CancellationToken>()), Times.Once);
        _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_MemberNotFound_ThrowsNotFoundException()
    {
        // Arrange
        _memberRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Member?)null);

        var command = new SuspendMemberCommand(Guid.NewGuid(), DateTime.UtcNow.AddDays(7), "Reason");

        // Act & Assert
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<NotFoundException>();
    }
}
