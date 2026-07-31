using LibraryMS.Application.Contracts.Borrows;
using LibraryMS.Application.Contracts.Common;
using LibraryMS.Application.Contracts.DTOs.Borrow;
using LibraryMS.Application.Mapping;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.BorrowManagement;
using LibraryMS.Domain.BorrowManagement.AggregateRoots;
using LibraryMS.Domain.BranchManagement;
using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.Shared.Guards;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Borrows;

public sealed class GetOverdueBorrowsQueryHandler : IRequestHandler<GetOverdueBorrowsQuery, PagedResult<BorrowDto>>
{
    private readonly IBorrowRepository _repository;
    private readonly IMemberRepository _memberRepo;
    private readonly IBookRepository _bookRepo;
    private readonly IBranchRepository _branchRepo;
    private readonly ILogger<GetOverdueBorrowsQueryHandler> _logger;

    public GetOverdueBorrowsQueryHandler(
        IBorrowRepository repository,
        IMemberRepository memberRepo,
        IBookRepository bookRepo,
        IBranchRepository branchRepo,
        ILogger<GetOverdueBorrowsQueryHandler> logger)
    {
        _repository = repository;
        _memberRepo = memberRepo;
        _bookRepo = bookRepo;
        _branchRepo = branchRepo;
        _logger = logger;
    }

    public async Task<PagedResult<BorrowDto>> Handle(GetOverdueBorrowsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving paged overdue borrows. Page: {Page}, PageSize: {PageSize}",
            request.Page, request.PageSize);

        Ensure.Against(request.Page < 1, "Page number must be greater than or equal to 1.", "INVALID_PAGE");
        Ensure.Against(request.PageSize < 1, "Page size must be greater than or equal to 1.", "INVALID_PAGE_SIZE");

        var (items, total) = await _repository.GetPagedAsync(
            null, null, "Overdue", request.Page, request.PageSize, cancellationToken);

        var dtos = await HydrateNavigationProperties(items, cancellationToken);

        _logger.LogInformation("Successfully retrieved {Count} overdue borrows out of {Total} total.", items.Count, total);

        return PagedResult<BorrowDto>.Create(dtos, total, request.Page, request.PageSize);
    }

    private async Task<List<BorrowDto>> HydrateNavigationProperties(List<BorrowRecord> items, CancellationToken ct)
    {
        if (items.Count == 0) return [];

        var memberIds = items.Select(i => i.MemberId).Distinct().ToList();
        var bookIds = items.Select(i => i.BookId).Distinct().ToList();
        var branchIds = items.Select(i => i.BranchId).Distinct().ToList();
        var copyIds = items.Select(i => i.BookCopyId).Distinct().ToList();

        var members = (await _memberRepo.GetByIdsAsync(memberIds, ct)).ToDictionary(m => m.Id);
        var books = (await _bookRepo.GetByIdsAsync(bookIds, ct)).ToDictionary(b => b.Id);
        var branches = (await _branchRepo.GetByIdsAsync(branchIds, ct)).ToDictionary(b => b.Id);

        var copies = books.Values
            .SelectMany(b => b.Copies)
            .Where(c => copyIds.Contains(c.Id))
            .ToDictionary(c => c.Id);

        return items.Select(record =>
            record.ToDto(
                members.GetValueOrDefault(record.MemberId),
                books.GetValueOrDefault(record.BookId),
                branches.GetValueOrDefault(record.BranchId),
                copies.GetValueOrDefault(record.BookCopyId)
            )
        ).ToList();
    }
}
