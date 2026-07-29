using LibraryMS.Domain.Shared.Exceptions;

namespace LibraryMS.Domain.Shared.Guards;

// Domain-Driven Design Guard Clauses to validate invariants and parameters.
public static class Ensure
{
    // Throws ValidationException if the given object is null.
    public static void NotNull<T>(T? obj, string message = "Value cannot be null.")
    {
        if (obj is null)
        {
            throw new ValidationException(message);
        }
    }

    // Throws ValidationException if the string is null, empty, or whitespace.
    public static void NotNullOrEmpty(string? value, string message = "Value cannot be null or empty.")
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ValidationException(message);
        }
    }

    // Throws NotFoundException if the requested entity was not found (is null).
    public static void Found<T>(T? entity, string message)
    {
        if (entity is null)
        {
            throw new NotFoundException(message);
        }
    }

    // Throws NotFoundException if the requested entity was not found (is null).
    public static void Found<T>(T? entity, string entityName, object key)
    {
        if (entity is null)
        {
            throw new NotFoundException(entityName, key);
        }
    }

    // Throws DomainException if a specific business rule is violated.
    public static void Against(bool ruleViolated, string message, string errorCode)
    {
        if (ruleViolated)
        {
            throw new DomainException(message, errorCode);
        }
    }

    // Throws UnauthorizedException if the assertion is false.
    public static void Authorized(bool condition, string message = "Invalid username or password.")
    {
        if (!condition)
        {
            throw new UnauthorizedException(message);
        }
    }

    // Throws ValidationException if any validation failures are present.
    public static void HasNoValidationFailures(IEnumerable<FluentValidation.Results.ValidationFailure> failures)
    {
        var failureList = failures as IList<FluentValidation.Results.ValidationFailure> ?? failures.ToList();
        if (failureList.Count > 0)
        {
            throw new ValidationException(failureList);
        }
    }
}
