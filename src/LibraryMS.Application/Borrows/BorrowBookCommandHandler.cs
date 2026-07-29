using LibraryMS.Application.Contracts.Borrows;
using LibraryMS.Application.Contracts.DTOs.Borrow;
using LibraryMS.Application.Mapping;
using LibraryMS.Domain.BorrowManagement;
using LibraryMS.Domain.BorrowManagement.AggregateRoots;
using LibraryMS.Domain.BorrowManagement.Services;
using LibraryMS.Domain.Shared;
using LibraryMS.Domain.Shared.Guards;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Borrows;

public sealed class BorrowBookCommandHandler : IRequestHandler<BorrowBookCommand, BorrowDto>
{
    private readonly BorrowManager _borrowManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BorrowBookCommandHandler> _logger;

    public BorrowBookCommandHandler(
        BorrowManager borrowManager,
        IUnitOfWork unitOfWork,
        ILogger<BorrowBookCommandHandler> logger)
    {
        _borrowManager = borrowManager;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<BorrowDto> Handle(BorrowBookCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing BorrowBookCommand for MemberId: {MemberId}, BookCopyId: {BookCopyId}, BookId: {BookId}, BranchId: {BranchId}",
            request.MemberId, request.BookCopyId, request.BookId, request.BranchId);

        var borrowDays = request.BorrowDays ?? BorrowRecord.MaxBorrowDays;

        var record = await _borrowManager.BorrowAsync(
            request.MemberId, request.BookCopyId, request.BookId,
            request.BranchId, borrowDays, cancellationToken);

        var dbFailed = false;
        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save borrow record for member {MemberId} and book copy {CopyId}.", request.MemberId, request.BookCopyId);
            dbFailed = true;
        }

        Ensure.Against(dbFailed, "An error occurred while saving the borrow transaction to the database.", "DB_UPDATE_ERROR");

        _logger.LogInformation(
            "Book copy {CopyId} successfully borrowed by member {MemberId}. Due: {DueDate:yyyy-MM-dd}",
            request.BookCopyId, request.MemberId, record.DueDate);

        return record.ToDto();
    }
}

