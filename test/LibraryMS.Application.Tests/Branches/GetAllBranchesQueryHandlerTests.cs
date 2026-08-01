using FluentAssertions;
using LibraryMS.Application.Branches;
using LibraryMS.Application.Contracts.Branches;
using LibraryMS.Application.Contracts.DTOs.Branch;
using LibraryMS.Domain.BranchManagement;
using LibraryMS.Domain.BranchManagement.AggregateRoots;
using LibraryMS.TestBase;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LibraryMS.Application.Tests.Branches;

public class GetAllBranchesQueryHandlerTests
{
    private readonly Mock<IBranchRepository> _branchRepoMock;
    private readonly Mock<ILogger<GetAllBranchesQueryHandler>> _loggerMock;
    
    private readonly GetAllBranchesQueryHandler _handler;

    public GetAllBranchesQueryHandlerTests()
    {
        _branchRepoMock = new Mock<IBranchRepository>();
        _loggerMock = new Mock<ILogger<GetAllBranchesQueryHandler>>();

        _handler = new GetAllBranchesQueryHandler(
            _branchRepoMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsListOfBranches()
    {
        // Arrange
        var branch1 = TestDataFactory.CreateBranch("Branch 1");
        var branch2 = TestDataFactory.CreateBranch("Branch 2");
        var list = new List<Branch> { branch1, branch2 };

        _branchRepoMock.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(list);

        var query = new GetAllBranchesQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(b => b.Name == "Branch 1");
        result.Should().Contain(b => b.Name == "Branch 2");
    }

    [Fact]
    public async Task Handle_EmptyRepository_ReturnsEmptyList()
    {
        // Arrange
        _branchRepoMock.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Branch>());

        var query = new GetAllBranchesQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}
