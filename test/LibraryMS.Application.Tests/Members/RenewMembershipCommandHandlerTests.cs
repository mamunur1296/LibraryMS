using FluentAssertions;
using LibraryMS.Application.Contracts.Members;
using LibraryMS.Application.Members;
using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.MemberManagement.AggregateRoots;
using LibraryMS.Domain.Shared;
using LibraryMS.Domain.Shared.Exceptions;
using LibraryMS.TestBase;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LibraryMS.Application.Tests.Members;

public class RenewMembershipCommandHandlerTests
{
    private readonly Mock<IMemberRepository> _memberRepoMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    
    private readonly RenewMembershipCommandHandler _handler;

    public RenewMembershipCommandHandlerTests()
    {
        _memberRepoMock = new Mock<IMemberRepository>();
        _uowMock = new Mock<IUnitOfWork>();

        _handler = new RenewMembershipCommandHandler(
            _memberRepoMock.Object,
            _uowMock.Object);
    }

    [Fact]
    public async Task Handle_ValidMember_RenewsMembership()
    {
        // Arrange
        var memberId = Guid.NewGuid();
        var member = TestDataFactory.CreateMember();
        var oldExpiry = member.MembershipExpiry;
        
        _memberRepoMock.Setup(x => x.GetByIdAsync(memberId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(member);

        var command = new RenewMembershipCommand(memberId, 1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        member.MembershipExpiry.Should().BeAfter(oldExpiry);
        
        _memberRepoMock.Verify(x => x.UpdateAsync(member, It.IsAny<CancellationToken>()), Times.Once);
        _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_MemberNotFound_ThrowsNotFoundException()
    {
        // Arrange
        _memberRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Member?)null);

        var command = new RenewMembershipCommand(Guid.NewGuid(), 1);

        // Act & Assert
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<NotFoundException>();
    }
}
