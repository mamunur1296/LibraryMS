namespace LibraryMS.Domain.Shared.Exceptions;

// Thrown when a requested entity does not exist.
// Maps to HTTP 404 Not Found.
public class NotFoundException : Exception
{
    public NotFoundException(string entityName, object key)
        : base($"{entityName} with key '{key}' was not found.")
    {
    }

    public NotFoundException(string message) : base(message)
    {
    }
}
