using LibraryMS.Application.Contracts.Common;
using LibraryMS.Application.Contracts.DTOs.Report;
using MediatR;

namespace LibraryMS.Application.Contracts.Reports;

// ──── Queries ────
public sealed record GetDashboardSummaryQuery()
    : IRequest<DashboardSummaryDto>;

public sealed record GetOverdueReportQuery(
    DateTime? FromDate, DateTime? ToDate,
    Guid? BranchId, int Page, int PageSize)
    : IRequest<PagedResult<OverdueReportDto>>;

public sealed record GetPopularBooksQuery(
    DateTime? FromDate, DateTime? ToDate,
    Guid? BranchId, int Page, int PageSize)
    : IRequest<PagedResult<PopularBookDto>>;

public sealed record GetMemberActivityReportQuery(
    DateTime? FromDate, DateTime? ToDate, int Page, int PageSize)
    : IRequest<PagedResult<MemberActivityDto>>;

public sealed record ExportOverdueReportQuery(
    DateTime? FromDate, DateTime? ToDate, Guid? BranchId, string Format = "excel")
    : IRequest<byte[]>;
