namespace LibraryMS.Domain.Shared.Exceptions;

/// <summary>
/// Thrown for authorization/permission failures.
/// Maps to HTTP 403 Forbidden.
/// </summary>
public class ForbiddenException : Exception
{
    public ForbiddenException(string message = "You do not have permission to perform this action.")
        : base(message)
    {
    }
}
