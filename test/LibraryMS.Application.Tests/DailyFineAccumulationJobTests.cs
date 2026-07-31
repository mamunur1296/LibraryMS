using LibraryMS.Application.BackgroundJobs;
using LibraryMS.Domain.BorrowManagement;
using LibraryMS.Domain.BorrowManagement.AggregateRoots;
using LibraryMS.Domain.Shared;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LibraryMS.Application.Tests.BackgroundJobs;

public class DailyFineAccumulationJobTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldCallBorrowManager()
    {
        // Arrange
        var mockBorrowRepo = new Mock<IBorrowRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockLogger = new Mock<ILogger<DailyFineAccumulationJob>>();
        
        mockBorrowRepo.Setup(r => r.GetOverdueBorrowsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BorrowRecord>());

        var job = new DailyFineAccumulationJob(mockBorrowRepo.Object, mockUnitOfWork.Object, mockLogger.Object);

        // Act
        await job.ExecuteAsync(CancellationToken.None);

        // Assert
        mockBorrowRepo.Verify(r => r.GetOverdueBorrowsAsync(It.IsAny<CancellationToken>()), Times.Once);
        mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
