using LibraryMS.Application.Contracts.Common;
using LibraryMS.Application.Contracts.DTOs.Report;
using LibraryMS.Application.Contracts.Reports;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryMS.HttpApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Librarian")]
public class ReportsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReportsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(DashboardSummaryDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboardSummary(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetDashboardSummaryQuery(), cancellationToken);
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
}
