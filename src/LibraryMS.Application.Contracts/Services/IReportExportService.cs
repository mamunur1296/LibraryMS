namespace LibraryMS.Application.Contracts.Services;

// Strategy Pattern interface for report exports.
// Implementations decide the output format (Excel vs PDF).
public interface IReportExportService
{
    Task<byte[]> ExportToExcelAsync<T>(IEnumerable<T> data, string sheetName, CancellationToken cancellationToken = default);
    Task<byte[]> ExportToPdfAsync(string title, string htmlContent, CancellationToken cancellationToken = default);
}
