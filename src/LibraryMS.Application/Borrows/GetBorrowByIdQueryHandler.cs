using LibraryMS.Application.Contracts.Borrows;
using LibraryMS.Application.Contracts.DTOs.Borrow;
using LibraryMS.Application.Mapping;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.BorrowManagement;
using LibraryMS.Domain.BranchManagement;
using LibraryMS.Domain.MemberManagement;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Borrows;

public sealed class GetBorrowByIdQueryHandler : IRequestHandler<GetBorrowByIdQuery, BorrowDto?>
{
    private readonly IBorrowRepository _repository;
    private readonly IMemberRepository _memberRepo;
    private readonly IBookRepository _bookRepo;
    private readonly IBranchRepository _branchRepo;
    private readonly ILogger<GetBorrowByIdQueryHandler> _logger;

    public GetBorrowByIdQueryHandler(
        IBorrowRepository repository,
        IMemberRepository memberRepo,
        IBookRepository bookRepo,
        IBranchRepository branchRepo,
        ILogger<GetBorrowByIdQueryHandler> logger)
    {
        _repository = repository;
        _memberRepo = memberRepo;
        _bookRepo = bookRepo;
        _branchRepo = branchRepo;
        _logger = logger;
    }

    public async Task<BorrowDto?> Handle(GetBorrowByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving borrow record with ID: {BorrowId}", request.Id);

        var record = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (record is null)
        {
            _logger.LogWarning("Borrow record with ID '{BorrowId}' was not found.", request.Id);
            return null;
        }

        var member = await _memberRepo.GetByIdAsync(record.MemberId, cancellationToken);
        var book = await _bookRepo.GetByIdAsync(record.BookId, cancellationToken);
        var branch = await _branchRepo.GetByIdAsync(record.BranchId, cancellationToken);
        var bookWithCopies = await _bookRepo.GetByIdWithCopiesAsync(record.BookId, cancellationToken);
        var copy = bookWithCopies?.Copies.FirstOrDefault(c => c.Id == record.BookCopyId);

        _logger.LogInformation("Successfully retrieved borrow record with ID: {BorrowId}", request.Id);

        return record.ToDto(member, book, branch, copy);
    }
}
