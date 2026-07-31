using LibraryMS.Application.Contracts.DTOs.Report;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryMS.Application.Contracts.Reports;

public interface IReportRepository
{
    Task<List<BranchComparisonDto>> GetBranchComparisonAsync(CancellationToken ct = default);
    Task<List<AnnualRevenueDto>> GetAnnualRevenueAsync(int year, CancellationToken ct = default);
    Task<List<MemberGrowthDto>> GetMemberGrowthAsync(int year, CancellationToken ct = default);
    Task<List<LibrarianActivityDto>> GetLibrarianActivityAsync(DateTime? fromDate, DateTime? toDate, CancellationToken ct = default);
}
