using FluentAssertions;
using LibraryMS.Application.Auth;
using LibraryMS.Application.Contracts.Auth;
using LibraryMS.Application.Contracts.Services;
using LibraryMS.Domain.Common;
using LibraryMS.Domain.IdentityManagement;
using LibraryMS.Domain.IdentityManagement.AggregateRoots;
using LibraryMS.Domain.IdentityManagement.Services;
using LibraryMS.Domain.Shared.Exceptions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LibraryMS.Application.Tests.Auth;

public class LoginCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IPasswordHasher> _hasherMock;
    private readonly Mock<IJwtTokenService> _jwtServiceMock;
    private readonly Mock<ILogger<LoginCommandHandler>> _loggerMock;
    private readonly Mock<IGuidGenerator> _guidGeneratorMock;
    
    private readonly UserManager _userManager;
    private readonly RefreshTokenManager _refreshTokenManager;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        var _branchRepoMock = new Mock<LibraryMS.Domain.BranchManagement.IBranchRepository>();
        _hasherMock = new Mock<IPasswordHasher>();
        _jwtServiceMock = new Mock<IJwtTokenService>();
        _loggerMock = new Mock<ILogger<LoginCommandHandler>>();

        _guidGeneratorMock = new Mock<IGuidGenerator>();
        _userManager = new UserManager(_guidGeneratorMock.Object);
        _refreshTokenManager = new RefreshTokenManager();

        _handler = new LoginCommandHandler(
            _userRepoMock.Object,
            _branchRepoMock.Object,
            _hasherMock.Object,
            _jwtServiceMock.Object,
            _loggerMock.Object,
            _userManager,
            _refreshTokenManager);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsAuthResponse()
    {
        // Arrange
        var user = Helpers.CreateUser(Guid.NewGuid(), "testuser", "test@test.com", "hash", "salt", LibraryMS.Domain.Shared.Enums.UserRole.Member);
        _userRepoMock.Setup(x => x.GetByUsernameAsync("testuser", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        
        _hasherMock.Setup(x => x.Verify("password123", "hash", "salt"))
            .Returns(true);
            
        _jwtServiceMock.Setup(x => x.GenerateAccessToken(user))
            .Returns(("access-token", DateTime.UtcNow.AddMinutes(60)));
            
        _jwtServiceMock.Setup(x => x.GenerateRefreshToken())
            .Returns("refresh-token");

        var command = new LoginCommand("testuser", "password123");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().Be("access-token");
        result.RefreshToken.Should().Be("refresh-token");
        
        _userRepoMock.Verify(x => x.UpdateAsync(It.Is<User>(u => u.LastLoginAt != null), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidPassword_ThrowsUnauthorizedException()
    {
        // Arrange
        var user = Helpers.CreateUser(Guid.NewGuid(), "testuser", "test@test.com", "hash", "salt", LibraryMS.Domain.Shared.Enums.UserRole.Member);
        _userRepoMock.Setup(x => x.GetByUsernameAsync("testuser", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        
        _hasherMock.Setup(x => x.Verify("wrongpass", "hash", "salt"))
            .Returns(false);

        var command = new LoginCommand("testuser", "wrongpass");

        // Act & Assert
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<UnauthorizedException>();
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsUnauthorizedException()
    {
        // Arrange
        _userRepoMock.Setup(x => x.GetByUsernameAsync("unknownuser", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
            
        _userRepoMock.Setup(x => x.GetByEmailAsync("unknownuser", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var command = new LoginCommand("unknownuser", "password123");

        // Act & Assert
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<UnauthorizedException>();
    }
}
