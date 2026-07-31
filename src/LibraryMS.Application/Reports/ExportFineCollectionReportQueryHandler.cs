using LibraryMS.Application.Contracts.Reports;
using LibraryMS.Application.Contracts.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text;

namespace LibraryMS.Application.Reports;

public sealed class ExportFineCollectionReportQueryHandler : IRequestHandler<ExportFineCollectionReportQuery, byte[]>
{
    private readonly IMediator _mediator;
    private readonly IReportExportService _exportService;
    private readonly ILogger<ExportFineCollectionReportQueryHandler> _logger;

    public ExportFineCollectionReportQueryHandler(
        IMediator mediator,
        IReportExportService exportService,
        ILogger<ExportFineCollectionReportQueryHandler> logger)
    {
        _mediator = mediator;
        _exportService = exportService;
        _logger = logger;
    }

    public async Task<byte[]> Handle(ExportFineCollectionReportQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Exporting Fine Collection Report in {Format} format.", request.Format);

        var data = await _mediator.Send(new GetFineCollectionReportQuery(request.FromDate, request.ToDate, request.BranchId), cancellationToken);

        if (request.Format.Equals("pdf", StringComparison.OrdinalIgnoreCase))
        {
            var htmlBuilder = new StringBuilder();
            htmlBuilder.Append("<h1>Fine Collection Report</h1>");
            htmlBuilder.Append("<table border='1' style='width:100%; border-collapse: collapse;'>");
            htmlBuilder.Append("<tr><th>Branch Name</th><th>Collected Fines</th><th>Pending Fines</th><th>No. Paid</th><th>No. Pending</th></tr>");
            foreach (var item in data)
            {
                htmlBuilder.Append($"<tr><td>{item.BranchName}</td><td>${item.TotalFinesCollected:F2}</td><td>${item.TotalPendingFines:F2}</td><td>{item.NumberOfFinesPaid}</td><td>{item.NumberOfPendingFines}</td></tr>");
            }
            htmlBuilder.Append("</table>");

            return await _exportService.ExportToPdfAsync("Fine Collection Report", htmlBuilder.ToString(), cancellationToken);
        }

        return await _exportService.ExportToExcelAsync(data, "Fine Collection", cancellationToken);
    }
}
