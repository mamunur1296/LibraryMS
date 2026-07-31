using FluentAssertions;
using LibraryMS.Application.Contracts.Users;
using LibraryMS.Application.Users;
using LibraryMS.Domain.IdentityManagement;
using LibraryMS.Domain.IdentityManagement.AggregateRoots;
using LibraryMS.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LibraryMS.Application.Tests.Users;

public class LibrarianManagementTests
{
    [Fact]
    public async Task Handle_ValidRequest_ShouldAssignBranch()
    {
        // Arrange
        var mockUserRepo = new Mock<IUserRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockLogger = new Mock<ILogger<AssignLibrarianToBranchCommandHandler>>();

        var userId = Guid.NewGuid();
        var branchId = Guid.NewGuid();

        var user = (User)Activator.CreateInstance(typeof(User), 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, 
            null, new object[] { userId, "librarian", "librarian@test.com", "hash", "salt", LibraryMS.Domain.Shared.Enums.UserRole.Librarian, null }, null)!;
        
        mockUserRepo.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var handler = new AssignLibrarianToBranchCommandHandler(
            mockUserRepo.Object,
            mockUnitOfWork.Object,
            mockLogger.Object);

        var command = new AssignLibrarianToBranchCommand(userId, branchId);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        user.BranchId.Should().Be(branchId);
        mockUserRepo.Verify(r => r.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }
}
