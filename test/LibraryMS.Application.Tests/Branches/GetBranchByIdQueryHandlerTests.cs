using FluentAssertions;
using LibraryMS.Application.Branches;
using LibraryMS.Application.Contracts.Branches;
using LibraryMS.Domain.BranchManagement;
using LibraryMS.Domain.BranchManagement.AggregateRoots;
using LibraryMS.TestBase;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LibraryMS.Application.Tests.Branches;

public class GetBranchByIdQueryHandlerTests
{
    private readonly Mock<IBranchRepository> _branchRepoMock;
    private readonly Mock<Microsoft.Extensions.Logging.ILogger<GetBranchByIdQueryHandler>> _loggerMock;
    private readonly GetBranchByIdQueryHandler _handler;

    public GetBranchByIdQueryHandlerTests()
    {
        _branchRepoMock = new Mock<IBranchRepository>();
        _loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<GetBranchByIdQueryHandler>>();
        _handler = new GetBranchByIdQueryHandler(_branchRepoMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_BranchExists_ReturnsBranch()
    {
        // Arrange
        var branch = TestDataFactory.CreateBranch();
        
        _branchRepoMock.Setup(r => r.GetByIdAsync(branch.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(branch);
            
        var query = new GetBranchByIdQuery(branch.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(branch.Id);
        result.Name.Should().Be(branch.Name);
    }

    [Fact]
    public async Task Handle_BranchDoesNotExist_ReturnsNull()
    {
        // Arrange
        _branchRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Branch?)null);
            
        var query = new GetBranchByIdQuery(Guid.NewGuid());

        var action = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<LibraryMS.Domain.Shared.Exceptions.NotFoundException>();
    }
}
