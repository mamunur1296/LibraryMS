using LibraryMS.Application.Contracts.Common;
using LibraryMS.Application.Contracts.DTOs.Report;
using LibraryMS.Application.Contracts.Reports;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryMS.HttpApi.Controllers;

[Authorize(Roles = "Admin,Librarian")]
public class ReportsController : BaseController
{
    private readonly IMediator _mediator;

    public ReportsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("dashboard-summary")]
    [Authorize(Roles = "Admin,Librarian")]
    public async Task<ActionResult<DashboardSummaryDto>> GetDashboardSummary(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetDashboardSummaryQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("admin-dashboard-summary")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AdminDashboardSummaryDto>> GetAdminDashboardSummary(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAdminDashboardSummaryQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("overdue")]
    [ProducesResponseType(typeof(PagedResult<OverdueReportDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOverdueReport(
        [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate,
        [FromQuery] Guid? branchId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetOverdueReportQuery(fromDate, toDate, branchId, page, pageSize), cancellationToken);
        return Ok(result);
    }

    [HttpGet("popular-books")]
    [ProducesResponseType(typeof(PagedResult<PopularBookDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPopularBooks(
        [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate,
        [FromQuery] Guid? branchId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetPopularBooksQuery(fromDate, toDate, branchId, page, pageSize), cancellationToken);
        return Ok(result);
    }

    [HttpGet("member-activity")]
    [ProducesResponseType(typeof(PagedResult<MemberActivityDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMemberActivity(
        [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetMemberActivityReportQuery(fromDate, toDate, page, pageSize), cancellationToken);
        return Ok(result);
    }

    [HttpGet("overdue/export")]
    public async Task<IActionResult> ExportOverdueReport(
        [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate,
        [FromQuery] Guid? branchId, [FromQuery] string format = "excel",
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new ExportOverdueReportQuery(fromDate, toDate, branchId, format), cancellationToken);
        var contentType = format.ToLower() == "pdf" ? "application/pdf" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        var fileName = $"OverdueReport_{DateTime.Now:yyyyMMdd}.{(format.ToLower() == "pdf" ? "pdf" : "xlsx")}";
        
        return File(result, contentType, fileName);
    }

    [HttpGet("fines/export")]
    public async Task<IActionResult> ExportFineCollectionReport(
        [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate,
        [FromQuery] Guid? branchId, [FromQuery] string format = "excel",
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new ExportFineCollectionReportQuery(fromDate, toDate, branchId, format), cancellationToken);
        var contentType = format.ToLower() == "pdf" ? "application/pdf" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        var fileName = $"FineCollectionReport_{DateTime.Now:yyyyMMdd}.{(format.ToLower() == "pdf" ? "pdf" : "xlsx")}";
        
        return File(result, contentType, fileName);
    }

    [HttpGet("branch-comparison")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(List<BranchComparisonDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBranchComparisonReport(CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetBranchComparisonReportQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("branch-comparison/export")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ExportBranchComparisonReport(
        [FromQuery] string format = "excel",
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new ExportBranchComparisonReportQuery(format), cancellationToken);
        var contentType = format.ToLower() == "pdf" ? "application/pdf" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        var fileName = $"BranchComparisonReport_{DateTime.Now:yyyyMMdd}.{(format.ToLower() == "pdf" ? "pdf" : "xlsx")}";
        
        return File(result, contentType, fileName);
    }

    [HttpGet("annual-revenue")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(List<AnnualRevenueDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAnnualRevenueReport([FromQuery] int year, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetAnnualRevenueReportQuery(year), cancellationToken);
        return Ok(result);
    }

    [HttpGet("fines")]
    [Authorize(Roles = "Admin,Librarian")]
    [ProducesResponseType(typeof(List<FineCollectionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFineCollectionReport(
        [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate,
        [FromQuery] Guid? branchId, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetFineCollectionReportQuery(fromDate, toDate, branchId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("member-growth")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(List<MemberGrowthDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMemberGrowthReport([FromQuery] int year, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetMemberGrowthReportQuery(year), cancellationToken);
        return Ok(result);
    }

    [HttpGet("librarian-activity")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(List<LibrarianActivityDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLibrarianActivityReport(
        [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetLibrarianActivityReportQuery(fromDate, toDate), cancellationToken);
        return Ok(result);
    }
}
