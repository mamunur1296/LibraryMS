using FluentAssertions;
using LibraryMS.Application.Contracts.Reservations;
using LibraryMS.Application.Reservations;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.BookManagement.AggregateRoots;
using LibraryMS.Domain.BorrowManagement;
using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.MemberManagement.AggregateRoots;
using LibraryMS.Domain.ReservationManagement;
using LibraryMS.Domain.Shared;
using LibraryMS.Domain.Shared.Exceptions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LibraryMS.Application.Tests.Reservations;

public class CreateReservationHandlerTests
{
    [Fact]
    public async Task Handle_WhenBookAvailable_ShouldThrowDomainException()
    {
        // Arrange
        var mockReservationRepo = new Mock<IReservationRepository>();
        var mockMemberRepo = new Mock<IMemberRepository>();
        var mockBookRepo = new Mock<IBookRepository>();
        var mockBorrowRepo = new Mock<IBorrowRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockLogger = new Mock<ILogger<CreateReservationCommandHandler>>();

        var memberId = Guid.NewGuid();
        var bookId = Guid.NewGuid();
        var branchId = Guid.NewGuid();

        var member = new Member(memberId, "Test", "Test", "test@test.com", "MEM-123", "123", "Address");
        var book = new Book(bookId, "Test Book", "1234567890", "Test", 2020, Guid.NewGuid(), Guid.NewGuid(), "Eng");
        book.AddCopy(branchId); // Book has an available copy

        mockMemberRepo.Setup(r => r.GetByIdAsync(memberId, It.IsAny<CancellationToken>())).ReturnsAsync(member);
        mockBookRepo.Setup(r => r.GetByIdWithCopiesAsync(bookId, It.IsAny<CancellationToken>())).ReturnsAsync(book);

        var handler = new CreateReservationCommandHandler(
            mockReservationRepo.Object,
            mockBookRepo.Object,
            mockMemberRepo.Object,
            mockBorrowRepo.Object,
            mockUnitOfWork.Object,
            mockLogger.Object);

        var command = new CreateReservationCommand(memberId, bookId, branchId);

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => handler.Handle(command, CancellationToken.None));
    }
}
