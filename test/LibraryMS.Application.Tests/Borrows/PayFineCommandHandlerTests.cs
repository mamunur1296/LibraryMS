using FluentAssertions;
using LibraryMS.Application.Borrows;
using LibraryMS.Application.Contracts.Borrows;
using LibraryMS.Domain.BorrowManagement;
using LibraryMS.Domain.BorrowManagement.Services;
using LibraryMS.Domain.Shared;
using LibraryMS.Domain.Shared.Exceptions;
using LibraryMS.TestBase;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LibraryMS.Application.Tests.Borrows;

public class PayFineCommandHandlerTests
{
    private readonly Mock<IBorrowRepository> _borrowRepoMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<ILogger<PayFineCommandHandler>> _loggerMock;
    
    private readonly PayFineCommandHandler _handler;

    public PayFineCommandHandlerTests()
    {
        _borrowRepoMock = new Mock<IBorrowRepository>();
        _uowMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<PayFineCommandHandler>>();

        _handler = new PayFineCommandHandler(
            _borrowRepoMock.Object,
            _uowMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidFinePayment_ReturnsTrue()
    {
        // Arrange
        var branch = TestDataFactory.CreateBranch();
        var book = TestDataFactory.CreateBook();
        var copy = book.AddCopy(branch.Id);
        var member = TestDataFactory.CreateMember();
        
        var borrow = TestDataFactory.CreateBorrowRecord(copy, member, DateTime.UtcNow);
        borrow.Return("Good");
        typeof(LibraryMS.Domain.BorrowManagement.AggregateRoots.BorrowRecord)
            .GetProperty("LateFine")
            ?.SetValue(borrow, 10.0m);

        var command = new PayFineCommand(borrow.Id);

        _borrowRepoMock.Setup(r => r.GetByIdAsync(borrow.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(borrow);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        borrow.IsFinePaid.Should().BeTrue();
        
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _borrowRepoMock.Verify(r => r.UpdateAsync(borrow, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_RecordNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var command = new PayFineCommand(Guid.NewGuid());

        _borrowRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((LibraryMS.Domain.BorrowManagement.AggregateRoots.BorrowRecord?)null);

        // Act & Assert
        var action = async () => await _handler.Handle(command, CancellationToken.None);
        
        await action.Should().ThrowAsync<NotFoundException>();
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
