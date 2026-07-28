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

public sealed class BorrowBookCommandHandler : IRequestHandler<BorrowBookCommand, BorrowDto>
{
    private readonly BorrowManager _borrowManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BorrowBookCommandHandler> _logger;

    public BorrowBookCommandHandler(
        BorrowManager borrowManager,
        IUnitOfWork unitOfWork, ILogger<BorrowBookCommandHandler> logger)
    {
        _borrowManager = borrowManager;
        _unitOfWork = unitOfWork; _logger = logger;
    }

    public async Task<BorrowDto> Handle(BorrowBookCommand request, CancellationToken cancellationToken)
    {
        var borrowDays = request.BorrowDays ?? BorrowRecord.MaxBorrowDays;

        var record = await _borrowManager.BorrowAsync(
            request.MemberId, request.BookCopyId, request.BookId,
            request.BranchId, borrowDays, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Book copy {CopyId} borrowed by member {MemberId}. Due: {DueDate:yyyy-MM-dd}",
            request.BookCopyId, request.MemberId, record.DueDate);

        return record.ToDto();
    }
}
