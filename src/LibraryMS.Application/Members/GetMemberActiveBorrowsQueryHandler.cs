using LibraryMS.Application.Contracts.DTOs.Borrow;
using LibraryMS.Application.Contracts.Members;
using LibraryMS.Application.Mapping;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.BorrowManagement;
using LibraryMS.Domain.BorrowManagement.AggregateRoots;
using LibraryMS.Domain.BranchManagement;
using LibraryMS.Domain.MemberManagement;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Members;

public sealed class GetMemberActiveBorrowsQueryHandler : IRequestHandler<GetMemberActiveBorrowsQuery, List<BorrowDto>>
{
    private readonly IBorrowRepository _borrowRepository;
    private readonly IMemberRepository _memberRepo;
    private readonly IBookRepository _bookRepo;
    private readonly IBranchRepository _branchRepo;
    private readonly ILogger<GetMemberActiveBorrowsQueryHandler> _logger;

    public GetMemberActiveBorrowsQueryHandler(
        IBorrowRepository borrowRepository,
        IMemberRepository memberRepo,
        IBookRepository bookRepo,
        IBranchRepository branchRepo,
        ILogger<GetMemberActiveBorrowsQueryHandler> logger)
    {
        _borrowRepository = borrowRepository;
        _memberRepo = memberRepo;
        _bookRepo = bookRepo;
        _branchRepo = branchRepo;
        _logger = logger;
    }

    public async Task<List<BorrowDto>> Handle(GetMemberActiveBorrowsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving active borrows for member {MemberId}", request.MemberId);

        var activeBorrows = await _borrowRepository.GetPagedAsync(request.MemberId, null, "Active", 1, 1000, cancellationToken);
        
        return await HydrateNavigationProperties(activeBorrows.Items, cancellationToken);
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
            MapperExtensions.ToDto(record,
                members.GetValueOrDefault(record.MemberId),
                books.GetValueOrDefault(record.BookId),
                branches.GetValueOrDefault(record.BranchId),
                copies.GetValueOrDefault(record.BookCopyId)
            )
        ).ToList();
    }
}
