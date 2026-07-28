using System;
using System.Collections.Generic;

namespace LibraryMS.Application.Contracts.Common;

/// <summary>
/// A standardized wrapper for all API responses.
/// </summary>
public sealed class ApiResponse<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public string Message { get; init; } = default!;
    public string? ErrorCode { get; init; }
    public Dictionary<string, string[]>? Errors { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    public static ApiResponse<T> SuccessResult(T data, string message = "Success")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    public static ApiResponse<T> FailureResult(string message, string? errorCode = null, Dictionary<string, string[]>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Data = default,
            Message = message,
            ErrorCode = errorCode,
            Errors = errors
        };
    }
}
