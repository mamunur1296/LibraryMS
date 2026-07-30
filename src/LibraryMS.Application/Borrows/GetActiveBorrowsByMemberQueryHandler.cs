using LibraryMS.Application.Contracts.Borrows;
using LibraryMS.Application.Contracts.DTOs.Borrow;
using LibraryMS.Application.Mapping;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.BranchManagement;
using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.BorrowManagement;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Borrows;

public sealed class GetActiveBorrowsByMemberQueryHandler : IRequestHandler<GetActiveBorrowsByMemberQuery, List<BorrowDto>>
{
    private readonly IBorrowRepository _repository;
    private readonly IMemberRepository _memberRepo;
    private readonly IBookRepository _bookRepo;
    private readonly IBranchRepository _branchRepo;
    private readonly ILogger<GetActiveBorrowsByMemberQueryHandler> _logger;

    public GetActiveBorrowsByMemberQueryHandler(
        IBorrowRepository repository,
        IMemberRepository memberRepo,
        IBookRepository bookRepo,
        IBranchRepository branchRepo,
        ILogger<GetActiveBorrowsByMemberQueryHandler> logger)
    {
        _repository = repository;
        _memberRepo = memberRepo;
        _bookRepo = bookRepo;
        _branchRepo = branchRepo;
        _logger = logger;
    }

    public async Task<List<BorrowDto>> Handle(GetActiveBorrowsByMemberQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving active borrows for MemberId: {MemberId}", request.MemberId);

        var records = await _repository.GetActiveBorrowsByMemberAsync(request.MemberId, cancellationToken);

        var bookIds = records.Select(r => r.BookId).Distinct().ToList();
        var branchIds = records.Select(r => r.BranchId).Distinct().ToList();
        var copyIds = records.Select(r => r.BookCopyId).Distinct().ToList();

        var member = await _memberRepo.GetByIdAsync(request.MemberId, cancellationToken);
        var books = (await _bookRepo.GetByIdsAsync(bookIds, cancellationToken)).ToDictionary(b => b.Id);
        var branches = (await _branchRepo.GetByIdsAsync(branchIds, cancellationToken)).ToDictionary(b => b.Id);

        var copies = books.Values
            .SelectMany(b => b.Copies)
            .Where(c => copyIds.Contains(c.Id))
            .ToDictionary(c => c.Id);

        _logger.LogInformation("Found {Count} active borrows for MemberId: {MemberId}", records.Count, request.MemberId);

        return records.Select(record =>
            record.ToDto(
                member,
                books.GetValueOrDefault(record.BookId),
                branches.GetValueOrDefault(record.BranchId),
                copies.GetValueOrDefault(record.BookCopyId)
            )
        ).ToList();
    }
}
