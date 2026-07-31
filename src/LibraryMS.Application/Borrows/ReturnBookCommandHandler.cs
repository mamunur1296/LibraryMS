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

public sealed class ReturnBookCommandHandler : IRequestHandler<ReturnBookCommand, BorrowDto>
{
    private readonly BorrowManager _borrowManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ReturnBookCommandHandler> _logger;

    public ReturnBookCommandHandler(
        BorrowManager borrowManager,
        IUnitOfWork unitOfWork,
        ILogger<ReturnBookCommandHandler> logger)
    {
        _borrowManager = borrowManager;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<BorrowDto> Handle(ReturnBookCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing ReturnBookCommand for BorrowId: {BorrowId}", request.BorrowId);

        var record = await _borrowManager.ReturnAsync(request.BorrowId, request.Notes, null, cancellationToken);

        var dbFailed = false;
        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save return record for borrow transaction {BorrowId}.", request.BorrowId);
            dbFailed = true;
        }

        Ensure.Against(dbFailed, "An error occurred while saving the return transaction to the database.", "DB_UPDATE_ERROR");

        _logger.LogInformation(
            "Borrow {BorrowId} successfully returned. Late fine: {Fine:C}",
            request.BorrowId, record.LateFine);

        return record.ToDto();
    }
}

