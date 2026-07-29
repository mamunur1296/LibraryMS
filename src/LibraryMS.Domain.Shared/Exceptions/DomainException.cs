namespace LibraryMS.Domain.Shared.Exceptions;

// Base exception for all domain business rule violations.
// Maps to HTTP 400 Bad Request.
public class DomainException : Exception
{
    public string Code { get; }

    public DomainException(string message, string code = "DOMAIN_ERROR")
        : base(message)
    {
        Code = code;
    }
}
