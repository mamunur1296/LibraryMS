using LibraryMS.Application.Mapping;
using LibraryMS.Application.Contracts.Borrows;
using LibraryMS.Application.Contracts.DTOs.Borrow;
using LibraryMS.Domain.BorrowManagement;
using LibraryMS.Domain.Shared;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryMS.Application.Borrows;

public sealed class ReturnBookCommandHandler : IRequestHandler<ReturnBookCommand, BorrowDto>
{
    private readonly BorrowManager _borrowManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ReturnBookCommandHandler> _logger;

    public ReturnBookCommandHandler(
        BorrowManager borrowManager,
        IUnitOfWork unitOfWork, ILogger<ReturnBookCommandHandler> logger)
    {
        _borrowManager = borrowManager;
        _unitOfWork = unitOfWork; _logger = logger;
    }

    public async Task<BorrowDto> Handle(ReturnBookCommand request, CancellationToken cancellationToken)
    {
        var record = await _borrowManager.ReturnAsync(request.BorrowId, request.Notes, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Borrow {BorrowId} returned. Late fine: {Fine:C}",
            request.BorrowId, record.LateFine);

        return record.ToDto();
    }
}
