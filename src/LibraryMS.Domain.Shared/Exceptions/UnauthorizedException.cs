namespace LibraryMS.Domain.Shared.Exceptions;

// Thrown for authentication failures.
// Maps to HTTP 401 Unauthorized.
public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message = "Authentication is required.")
        : base(message)
    {
    }
}
