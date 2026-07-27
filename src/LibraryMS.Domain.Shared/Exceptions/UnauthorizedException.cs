namespace LibraryMS.Domain.Shared.Exceptions;

/// <summary>
/// Thrown for authentication failures.
/// Maps to HTTP 401 Unauthorized.
/// </summary>
public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message = "Authentication is required.")
        : base(message)
    {
    }
}
