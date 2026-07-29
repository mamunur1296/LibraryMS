using LibraryMS.Application.Contracts.Common;
using LibraryMS.Application.Contracts.DTOs.Report;
using LibraryMS.Application.Contracts.Reports;
using LibraryMS.Domain.BorrowManagement;
using LibraryMS.Domain.MemberManagement;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

        var items = new List<MemberActivityDto>();

        foreach (var m in members)
        {
            var (borrows, _) = await _borrowRepo.GetPagedAsync(
                m.Id, null, null, 1, int.MaxValue, cancellationToken);

            var query = borrows.AsQueryable();

            if (request.FromDate.HasValue)
                query = query.Where(b => b.BorrowDate >= request.FromDate.Value);

            if (request.ToDate.HasValue)
                query = query.Where(b => b.BorrowDate <= request.ToDate.Value);

            var filteredBorrows = query.ToList();

            items.Add(new MemberActivityDto
            {
                MemberId = m.Id,
                FullName = $"{m.FirstName} {m.LastName}",
                MembershipNumber = m.MembershipNumber,
                TotalBorrows = filteredBorrows.Count,
                ActiveBorrows = filteredBorrows.Count(b => b.Status == Domain.Shared.Enums.BorrowStatus.Active),
                OverdueBorrows = filteredBorrows.Count(b => b.Status == Domain.Shared.Enums.BorrowStatus.Overdue),
                TotalFinesPaid = filteredBorrows.Where(b => b.IsFinePaid).Sum(b => b.LateFine)
            });
        }

        return PagedResult<MemberActivityDto>.Create(items, totalCount, request.Page, request.PageSize);
    }
}
