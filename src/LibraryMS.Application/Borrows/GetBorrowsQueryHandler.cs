using LibraryMS.Application.Contracts.Borrows;
using LibraryMS.Application.Contracts.Common;
using LibraryMS.Application.Contracts.DTOs.Borrow;
using LibraryMS.Application.Mapping;
using LibraryMS.Domain.BorrowManagement;
using LibraryMS.Domain.Shared.Guards;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Borrows;

public sealed class GetBorrowsQueryHandler : IRequestHandler<GetBorrowsQuery, PagedResult<BorrowDto>>
{
    private readonly IBorrowRepository _repository;
    private readonly ILogger<GetBorrowsQueryHandler> _logger;

    public GetBorrowsQueryHandler(IBorrowRepository repository, ILogger<GetBorrowsQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<PagedResult<BorrowDto>> Handle(GetBorrowsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving paged borrow records. MemberId: {MemberId}, BookId: {BookId}, Status: {Status}, Page: {Page}, PageSize: {PageSize}",
            request.MemberId, request.BookId, request.Status, request.Page, request.PageSize);

        Ensure.Against(request.Page < 1, "Page number must be greater than or equal to 1.", "INVALID_PAGE");
        Ensure.Against(request.PageSize < 1, "Page size must be greater than or equal to 1.", "INVALID_PAGE_SIZE");

        var (items, total) = await _repository.GetPagedAsync(
            request.MemberId, request.BookId, request.Status,
            request.Page, request.PageSize, cancellationToken);

        _logger.LogInformation("Successfully retrieved {Count} borrow records out of {Total} total.", items.Count, total);

        return PagedResult<BorrowDto>.Create(
            items.Select(i => i.ToDto()).ToList(),
            total, request.Page, request.PageSize);
    }
}
