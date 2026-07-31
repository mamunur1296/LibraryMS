using LibraryMS.Application.Contracts.Borrows;
using LibraryMS.Application.Contracts.DTOs.Borrow;
using LibraryMS.Application.Mapping;
using LibraryMS.Domain.BorrowManagement;
using LibraryMS.Domain.BorrowManagement.AggregateRoots;
using LibraryMS.Domain.Shared;
using LibraryMS.Domain.Shared.Guards;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Borrows;

public sealed class PayFineCommandHandler : IRequestHandler<PayFineCommand, BorrowDto>
{
    private readonly IBorrowRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PayFineCommandHandler> _logger;

    public PayFineCommandHandler(
        IBorrowRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<PayFineCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<BorrowDto> Handle(PayFineCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing PayFineCommand for BorrowId: {BorrowId}", request.BorrowId);

        var record = await _repository.GetByIdAsync(request.BorrowId, cancellationToken);
        Ensure.Found(record, $"Borrow record with ID '{request.BorrowId}' was not found.");

        record!.PayFine();
        await _repository.UpdateAsync(record, cancellationToken);

        var dbFailed = false;
        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save fine payment for borrow {BorrowId}.", request.BorrowId);
            dbFailed = true;
        }

        Ensure.Against(dbFailed, "An error occurred while processing the fine payment.", "DB_UPDATE_ERROR");

        _logger.LogInformation("Fine payment processed for borrow {BorrowId}. Fine: {Fine:C}", request.BorrowId, record.LateFine);

        return record.ToDto();
    }
}
