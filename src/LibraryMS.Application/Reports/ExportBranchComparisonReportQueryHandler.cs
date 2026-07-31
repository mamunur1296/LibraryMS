using LibraryMS.Application.Contracts.Reports;
using LibraryMS.Application.Contracts.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text;

namespace LibraryMS.Application.Reports;

public sealed class ExportBranchComparisonReportQueryHandler : IRequestHandler<ExportBranchComparisonReportQuery, byte[]>
{
    private readonly IMediator _mediator;
    private readonly IReportExportService _exportService;
    private readonly ILogger<ExportBranchComparisonReportQueryHandler> _logger;

    public ExportBranchComparisonReportQueryHandler(
        IMediator mediator,
        IReportExportService exportService,
        ILogger<ExportBranchComparisonReportQueryHandler> logger)
    {
        _mediator = mediator;
        _exportService = exportService;
        _logger = logger;
    }

    public async Task<byte[]> Handle(ExportBranchComparisonReportQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Exporting Branch Comparison Report in {Format} format.", request.Format);

        var data = await _mediator.Send(new GetBranchComparisonReportQuery(), cancellationToken);

        if (request.Format.Equals("pdf", StringComparison.OrdinalIgnoreCase))
        {
            var htmlBuilder = new StringBuilder();
            htmlBuilder.Append("<h1>Branch Comparison Report</h1>");
            htmlBuilder.Append("<table border='1' style='width:100%; border-collapse: collapse;'>");
            htmlBuilder.Append("<tr><th>Branch Name</th><th>Total Books</th><th>Active Borrows</th><th>Overdue Borrows</th><th>Total Revenue ($)</th></tr>");
            foreach (var item in data)
            {
                htmlBuilder.Append($"<tr><td>{item.BranchName}</td><td>{item.TotalBooks}</td><td>{item.ActiveBorrows}</td><td>{item.OverdueBorrows}</td><td>{item.TotalRevenue:C}</td></tr>");
            }
            htmlBuilder.Append("</table>");

            return await _exportService.ExportToPdfAsync("Branch Comparison Report", htmlBuilder.ToString(), cancellationToken);
        }

        return await _exportService.ExportToExcelAsync(data, "Branch Comparison", cancellationToken);
    }
}
