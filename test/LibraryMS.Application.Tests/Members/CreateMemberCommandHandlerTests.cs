using FluentAssertions;
using LibraryMS.Application.Contracts.Members;
using LibraryMS.Application.Contracts.Services;
using LibraryMS.Application.Members;
using LibraryMS.Domain.Common;
using LibraryMS.Domain.IdentityManagement;
using LibraryMS.Domain.IdentityManagement.AggregateRoots;
using LibraryMS.Domain.IdentityManagement.Services;
using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.MemberManagement.AggregateRoots;
using LibraryMS.Domain.MemberManagement.Services;
using LibraryMS.Domain.Shared;
using Microsoft.Extensions.Logging;
using Moq;

namespace LibraryMS.Application.Tests.Members;

public class CreateMemberCommandHandlerTests
{
    private readonly Mock<IMemberRepository> _memberRepoMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<ILogger<CreateMemberCommandHandler>> _loggerMock;
    private readonly Mock<IGuidGenerator> _guidGeneratorMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IPasswordHasher> _hasherMock;

    private readonly MemberManager _memberManager;
    private readonly UserManager _userManager;
    private readonly CreateMemberCommandHandler _handler;

    public CreateMemberCommandHandlerTests()
    {
        _memberRepoMock = new Mock<IMemberRepository>();
        _uowMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<CreateMemberCommandHandler>>();
        _userRepoMock = new Mock<IUserRepository>();
        _hasherMock = new Mock<IPasswordHasher>();

        _memberManager = new MemberManager(_memberRepoMock.Object);
        _guidGeneratorMock = new Mock<IGuidGenerator>();
        _userManager = new UserManager(_guidGeneratorMock.Object);

        _handler = new CreateMemberCommandHandler(
            _memberManager,
            _memberRepoMock.Object,
            _uowMock.Object,
            _loggerMock.Object,
            _userRepoMock.Object,
            _hasherMock.Object,
            _userManager);
    }

    [Fact]
    public async Task Handle_ValidCommandWithoutUser_CreatesMember()
    {
        // Arrange
        var command = new CreateMemberCommand(
            "John", "Doe", "john@test.com", "555-1234", "123 Main St", null, null);

        _memberRepoMock.Setup(x => x.EmailExistsAsync("john@test.com", It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.FirstName.Should().Be("John");
        result.Email.Should().Be("john@test.com");

        _memberRepoMock.Verify(x => x.AddAsync(It.Is<Member>(m => m.Email == "john@test.com"), It.IsAny<CancellationToken>()), Times.Once);
        _userRepoMock.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommandWithUser_CreatesMemberAndUser()
    {
        // Arrange
        var command = new CreateMemberCommand(
            "Jane", "Doe", "jane@test.com", "555-1234", "123 Main St", "janedoe", "Password123!");

        _memberRepoMock.Setup(x => x.EmailExistsAsync("jane@test.com", It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _userRepoMock.Setup(x => x.UsernameExistsAsync("janedoe", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _userRepoMock.Setup(x => x.EmailExistsAsync("jane@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _hasherMock.Setup(x => x.Hash("Password123!"))
            .Returns(("hash", "salt"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();

        _memberRepoMock.Verify(x => x.AddAsync(It.Is<Member>(m => m.Email == "jane@test.com"), It.IsAny<CancellationToken>()), Times.Once);
        _userRepoMock.Verify(x => x.AddAsync(It.Is<User>(u => u.Username == "janedoe"), It.IsAny<CancellationToken>()), Times.Once);
        _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
