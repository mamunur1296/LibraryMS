namespace LibraryMS.Application.Contracts.Common;

/// <summary>Standard API error response.</summary>
public sealed class ApiErrorResponse
{
    public string Type { get; init; } = default!;
    public string Message { get; init; } = default!;
    public string? Code { get; init; }
    public Dictionary<string, string[]>? ValidationErrors { get; init; }
    public string TraceId { get; init; } = default!;
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}
