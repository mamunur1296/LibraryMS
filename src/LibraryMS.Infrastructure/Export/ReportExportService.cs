using ClosedXML.Excel;
using LibraryMS.Application.Contracts.Services;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Reflection;

namespace LibraryMS.Infrastructure.Export;

/// <summary>
/// Implements both Excel (ClosedXML) and PDF (QuestPDF) report generation.
/// Strategy Pattern: caller decides which format to invoke.
/// </summary>
public sealed class ReportExportService : IReportExportService
{
    /// <summary>
    /// Generates an Excel (.xlsx) file from any list of objects using reflection.
    /// </summary>
    public Task<byte[]> ExportToExcelAsync<T>(
        IEnumerable<T> data, string sheetName, CancellationToken cancellationToken = default)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add(sheetName);

        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        // Header row
        for (var i = 0; i < properties.Length; i++)
        {
            var cell = worksheet.Cell(1, i + 1);
            cell.Value = properties[i].Name;
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#4472C4");
            cell.Style.Font.FontColor = XLColor.White;
        }

        // Data rows
        var rowIndex = 2;
        foreach (var item in data)
        {
            for (var i = 0; i < properties.Length; i++)
            {
                var value = properties[i].GetValue(item);
                worksheet.Cell(rowIndex, i + 1).Value = value?.ToString() ?? string.Empty;
            }
            rowIndex++;
        }

        worksheet.Columns().AdjustToContents();
        worksheet.SheetView.FreezeRows(1); // Freeze header row

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return Task.FromResult(stream.ToArray());
    }

    /// <summary>
    /// Generates a PDF report using QuestPDF (Community License).
    /// </summary>
    public Task<byte[]> ExportToPdfAsync(
        string title, string htmlContent, CancellationToken cancellationToken = default)
    {
        var pdfBytes = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(40);

                page.Header()
                    .Text(title)
                    .FontSize(18)
                    .Bold()
                    .FontColor(Color.FromHex("#1E40AF"));

                page.Content()
                    .PaddingTop(10)
                    .Column(col =>
                    {
                        col.Item()
                            .Text(htmlContent)
                            .FontSize(10)
                            .LineHeight(1.4f);
                    });

                page.Footer()
                    .AlignRight()
                    .Text(text =>
                    {
                        text.Span("Page ").FontSize(9);
                        text.CurrentPageNumber().FontSize(9);
                        text.Span(" of ").FontSize(9);
                        text.TotalPages().FontSize(9);
                    });
            });
        }).GeneratePdf();

        return Task.FromResult(pdfBytes);
    }
}
