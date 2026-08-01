using FluentAssertions;
using LibraryMS.Application.Contracts.Services;
using LibraryMS.Application.Contracts.Users;
using LibraryMS.Application.Users;
using LibraryMS.Domain.IdentityManagement;
using LibraryMS.Domain.IdentityManagement.AggregateRoots;
using LibraryMS.Domain.Shared;
using LibraryMS.Domain.Shared.Exceptions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LibraryMS.Application.Tests.Users;

public class ChangePasswordCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IPasswordHasher> _hasherMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<ILogger<ChangePasswordCommandHandler>> _loggerMock;
    
    private readonly ChangePasswordCommandHandler _handler;

    public ChangePasswordCommandHandlerTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _hasherMock = new Mock<IPasswordHasher>();
        _uowMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<ChangePasswordCommandHandler>>();

        _handler = new ChangePasswordCommandHandler(
            _userRepoMock.Object,
            _hasherMock.Object,
            _uowMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ChangesPasswordAndSaves()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = Helpers.CreateUser(userId, "testuser", "test@test.com", "oldHash", "oldSalt", LibraryMS.Domain.Shared.Enums.UserRole.Member);
        
        _userRepoMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
            
        _hasherMock.Setup(x => x.Verify("oldPassword", "oldHash", "oldSalt"))
            .Returns(true);
            
        _hasherMock.Setup(x => x.Hash("newPassword"))
            .Returns(("newHash", "newSalt"));

        var command = new ChangePasswordCommand(userId, "oldPassword", "newPassword");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        user.PasswordHash.Should().Be("newHash");
        user.PasswordSalt.Should().Be("newSalt");
        
        _userRepoMock.Verify(x => x.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
        _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_IncorrectCurrentPassword_ThrowsUnauthorizedException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = Helpers.CreateUser(userId, "testuser", "test@test.com", "oldHash", "oldSalt", LibraryMS.Domain.Shared.Enums.UserRole.Member);
        
        _userRepoMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
            
        _hasherMock.Setup(x => x.Verify("wrongOldPassword", "oldHash", "oldSalt"))
            .Returns(false);

        var command = new ChangePasswordCommand(userId, "wrongOldPassword", "newPassword");

        // Act & Assert
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<UnauthorizedException>();
        
        _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userRepoMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var command = new ChangePasswordCommand(userId, "oldPassword", "newPassword");

        // Act & Assert
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<NotFoundException>();
    }
}
