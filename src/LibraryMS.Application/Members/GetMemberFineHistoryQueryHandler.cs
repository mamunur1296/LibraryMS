using LibraryMS.Application.Contracts.DTOs.Borrow;
using LibraryMS.Application.Contracts.Members;
using LibraryMS.Domain.BorrowManagement;
using LibraryMS.Domain.BorrowManagement.AggregateRoots;
using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.BranchManagement;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Members;

public sealed class GetMemberFineHistoryQueryHandler : IRequestHandler<GetMemberFineHistoryQuery, List<BorrowDto>>
{
    private readonly IBorrowRepository _borrowRepository;
    private readonly IMemberRepository _memberRepo;
    private readonly IBookRepository _bookRepo;
    private readonly IBranchRepository _branchRepo;
    private readonly ILogger<GetMemberFineHistoryQueryHandler> _logger;

    public GetMemberFineHistoryQueryHandler(
        IBorrowRepository borrowRepository,
        IMemberRepository memberRepo,
        IBookRepository bookRepo,
        IBranchRepository branchRepo,
        ILogger<GetMemberFineHistoryQueryHandler> logger)
    {
        _borrowRepository = borrowRepository;
        _memberRepo = memberRepo;
        _bookRepo = bookRepo;
        _branchRepo = branchRepo;
        _logger = logger;
    }

    public async Task<List<BorrowDto>> Handle(GetMemberFineHistoryQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving fine history for member {MemberId}", request.MemberId);

        // We fetch all borrows for the member that have a fine > 0
        var allBorrows = await _borrowRepository.GetPagedAsync(request.MemberId, null, null, 1, 1000, cancellationToken);
        var fines = allBorrows.Items.Where(b => b.LateFine > 0).ToList();
        
        return await HydrateNavigationProperties(fines, cancellationToken);
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
            LibraryMS.Application.Mapping.MapperExtensions.ToDto(record,
                members.GetValueOrDefault(record.MemberId),
                books.GetValueOrDefault(record.BookId),
                branches.GetValueOrDefault(record.BranchId),
                copies.GetValueOrDefault(record.BookCopyId)
            )
        ).ToList();
    }
}
