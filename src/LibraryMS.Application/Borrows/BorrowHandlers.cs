using LibraryMS.Application.Mapping;
using LibraryMS.Application.Contracts.Borrows;
using LibraryMS.Application.Contracts.Common;
using LibraryMS.Application.Contracts.DTOs.Borrow;
using LibraryMS.Domain.BorrowManagement;
using LibraryMS.Domain.Shared.Exceptions;
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

public sealed class GetBorrowsQueryHandler : IRequestHandler<GetBorrowsQuery, PagedResult<BorrowDto>>
{
    private readonly IBorrowRepository _repository;
    

    public GetBorrowsQueryHandler(IBorrowRepository repository)
    {
        _repository = repository; 
    }

    public async Task<PagedResult<BorrowDto>> Handle(GetBorrowsQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await _repository.GetPagedAsync(
            request.MemberId, request.BookId, request.Status,
            request.Page, request.PageSize, cancellationToken);

        return PagedResult<BorrowDto>.Create(
            items.Select(i => i.ToDto()).ToList(),
            total, request.Page, request.PageSize);
    }
}

public sealed class GetBorrowByIdQueryHandler : IRequestHandler<GetBorrowByIdQuery, BorrowDto?>
{
    private readonly IBorrowRepository _repository;
    

    public GetBorrowByIdQueryHandler(IBorrowRepository repository)
    {
        _repository = repository; 
    }

    public async Task<BorrowDto?> Handle(GetBorrowByIdQuery request, CancellationToken cancellationToken)
    {
        var record = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return record?.ToDto();
    }
}
