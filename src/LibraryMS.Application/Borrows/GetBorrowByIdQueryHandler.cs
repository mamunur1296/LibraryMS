using LibraryMS.Application.Contracts.Borrows;
using LibraryMS.Application.Contracts.DTOs.Borrow;
using LibraryMS.Application.Mapping;
using LibraryMS.Domain.BorrowManagement;
using LibraryMS.Domain.BorrowManagement.AggregateRoots;
using LibraryMS.Domain.BorrowManagement.Services;
using LibraryMS.Domain.Shared.Guards;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Borrows;

public sealed class GetBorrowByIdQueryHandler : IRequestHandler<GetBorrowByIdQuery, BorrowDto?>
{
    private readonly IBorrowRepository _repository;
    private readonly ILogger<GetBorrowByIdQueryHandler> _logger;

    public GetBorrowByIdQueryHandler(IBorrowRepository repository, ILogger<GetBorrowByIdQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<BorrowDto?> Handle(GetBorrowByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving borrow record with ID: {BorrowId}", request.Id);

        var record = await _repository.GetByIdAsync(request.Id, cancellationToken);

        Ensure.Found(record, $"Borrow record with ID '{request.Id}' was not found.");

        _logger.LogInformation("Successfully retrieved borrow record with ID: {BorrowId}", request.Id);

        return record?.ToDto();
    }
}

