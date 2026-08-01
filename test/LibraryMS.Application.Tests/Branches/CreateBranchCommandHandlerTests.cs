using FluentAssertions;
using LibraryMS.Application.Branches;
using LibraryMS.Application.Contracts.Branches;
using LibraryMS.Application.Contracts.DTOs.Branch;
using LibraryMS.Domain.BranchManagement;
using LibraryMS.Domain.BranchManagement.AggregateRoots;
using LibraryMS.Domain.BranchManagement.Services;
using LibraryMS.Domain.Shared;
using LibraryMS.Domain.Shared.Exceptions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LibraryMS.Application.Tests.Branches;

public class CreateBranchCommandHandlerTests
{
    private readonly Mock<IBranchRepository> _branchRepoMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<ILogger<CreateBranchCommandHandler>> _loggerMock;
    
    private readonly BranchManager _branchManager;
    private readonly CreateBranchCommandHandler _handler;

    public CreateBranchCommandHandlerTests()
    {
        _branchRepoMock = new Mock<IBranchRepository>();
        _uowMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<CreateBranchCommandHandler>>();
        
        _branchManager = new BranchManager(_branchRepoMock.Object);

        _handler = new CreateBranchCommandHandler(
            _branchManager,
            _branchRepoMock.Object,
            _uowMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesBranchAndSaves()
    {
        // Arrange
        var command = new CreateBranchCommand(
            "Central Library", "123 Main St", "555-1234", "contact@central.lib");

        _branchRepoMock.Setup(x => x.ExistsWithNameAsync("Central Library", It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Central Library");
        
        _branchRepoMock.Verify(x => x.AddAsync(It.Is<Branch>(b => b.Name == "Central Library"), It.IsAny<CancellationToken>()), Times.Once);
        _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DuplicateName_ThrowsDomainException()
    {
        // Arrange
        var command = new CreateBranchCommand(
            "Central Library", "123 Main St", "555-1234", "contact@central.lib");

        _branchRepoMock.Setup(x => x.ExistsWithNameAsync("Central Library", It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        await action.Should().ThrowAsync<DomainException>()
            .WithMessage("*A branch named*already exists*");
            
        _branchRepoMock.Verify(x => x.AddAsync(It.IsAny<Branch>(), It.IsAny<CancellationToken>()), Times.Never);
        _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
