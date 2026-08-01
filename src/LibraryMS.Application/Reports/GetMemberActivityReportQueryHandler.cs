using LibraryMS.Application.Contracts.Common;
using LibraryMS.Application.Contracts.DTOs.Report;
using LibraryMS.Application.Contracts.Reports;
using LibraryMS.Domain.BorrowManagement;
using LibraryMS.Domain.BorrowManagement.AggregateRoots;
using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.Shared.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Reports;

public sealed class GetMemberActivityReportQueryHandler : IRequestHandler<GetMemberActivityReportQuery, PagedResult<MemberActivityDto>>
{
    private readonly IMemberRepository _memberRepo;
    private readonly IBorrowRepository _borrowRepo;
    private readonly ILogger<GetMemberActivityReportQueryHandler> _logger;

    public GetMemberActivityReportQueryHandler(
        IMemberRepository memberRepo,
        IBorrowRepository borrowRepo,
        ILogger<GetMemberActivityReportQueryHandler> logger)
    {
        _memberRepo = memberRepo;
        _borrowRepo = borrowRepo;
        _logger = logger;
    }

    public async Task<PagedResult<MemberActivityDto>> Handle(GetMemberActivityReportQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating Member Activity Report via Domain Repositories.");

        var (members, totalCount) = await _memberRepo.SearchAsync(
            null, null, request.Page, request.PageSize, cancellationToken);

        var memberIds = members.Select(m => m.Id).ToList();
        var allBorrows = memberIds.Count > 0
            ? await _borrowRepo.GetByMemberIdsAsync(memberIds, request.FromDate, request.ToDate, cancellationToken)
            : new List<BorrowRecord>();

        var borrowsByMember = allBorrows.GroupBy(b => b.MemberId).ToDictionary(g => g.Key, g => g.ToList());

        var items = members.Select(m =>
        {
            var memberBorrows = borrowsByMember.GetValueOrDefault(m.Id, new List<BorrowRecord>());

            return new MemberActivityDto
            {
                MemberId = m.Id,
                FullName = $"{m.FirstName} {m.LastName}",
                MembershipNumber = m.MembershipNumber,
                TotalBorrows = memberBorrows.Count,
                ActiveBorrows = memberBorrows.Count(b => b.Status == BorrowStatus.Active),
                OverdueBorrows = memberBorrows.Count(b => b.Status == BorrowStatus.Overdue),
                TotalFinesPaid = memberBorrows.Where(b => b.IsFinePaid).Sum(b => b.LateFine)
            };
        }).ToList();

        return PagedResult<MemberActivityDto>.Create(items, totalCount, request.Page, request.PageSize);
    }
}
