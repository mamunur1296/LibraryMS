namespace LibraryMS.Application.Contracts.Services;

/// <summary>
/// Adapter Pattern interface for Email sending.
/// Application layer depends on this abstraction, not on MailKit directly.
/// </summary>
public interface IEmailService
{
    Task SendAsync(string toEmail, string toName, string subject, string htmlBody, CancellationToken cancellationToken = default);
}

/// <summary>
/// Adapter Pattern interface for distributed caching (Redis).
/// </summary>
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default);
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default);
}

/// <summary>
/// Strategy Pattern interface for report exports.
/// Implementations decide the output format (Excel vs PDF).
/// </summary>
public interface IReportExportService
{
    Task<byte[]> ExportToExcelAsync<T>(IEnumerable<T> data, string sheetName, CancellationToken cancellationToken = default);
    Task<byte[]> ExportToPdfAsync(string title, string htmlContent, CancellationToken cancellationToken = default);
}
