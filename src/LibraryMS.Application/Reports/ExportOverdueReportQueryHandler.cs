using LibraryMS.Application.Contracts.Reports;
using LibraryMS.Application.Contracts.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryMS.Application.Reports;

public sealed class ExportOverdueReportQueryHandler : IRequestHandler<ExportOverdueReportQuery, byte[]>
{
    private readonly IMediator _mediator;
    private readonly IReportExportService _exportService;
    private readonly ILogger<ExportOverdueReportQueryHandler> _logger;

    public ExportOverdueReportQueryHandler(
        IMediator mediator,
        IReportExportService exportService,
        ILogger<ExportOverdueReportQueryHandler> logger)
    {
        _mediator = mediator;
        _exportService = exportService;
        _logger = logger;
    }

    public async Task<byte[]> Handle(ExportOverdueReportQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Exporting Overdue Report format: {Format} via Application abstraction", request.Format);

        // Fetch all overdue records (using large page size to export all records)
        var reportData = await _mediator.Send(
            new GetOverdueReportQuery(request.FromDate, request.ToDate, request.BranchId, 1, 10000), cancellationToken);

        if (request.Format.ToLower() == "pdf")
        {
            var textBody = $"Overdue Report generated on {DateTime.Now:g}.\nTotal Records: {reportData.TotalCount}\n\n";
            foreach (var item in reportData.Items)
            {
                textBody += $"Member: {item.MemberName} ({item.MembershipNumber}) - Book: {item.BookTitle} - Due: {item.DueDate:d} - Overdue: {item.OverdueDays} days - Fine: ${item.AccruedFine}\n";
            }
            return await _exportService.ExportToPdfAsync("Overdue Book Report", textBody, cancellationToken);
        }
        else
        {
            // Excel Export
            return await _exportService.ExportToExcelAsync(reportData.Items, "Overdue Report", cancellationToken);
        }
    }
}
