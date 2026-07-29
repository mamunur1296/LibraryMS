using LibraryMS.Domain.Shared.Exceptions;

namespace LibraryMS.Domain.Shared.Guards;

/// <summary>
/// Domain-Driven Design Guard Clauses to validate invariants and parameters.
/// </summary>
public static class Ensure
{
    /// <summary>
    /// Throws ValidationException if the given object is null.
    /// </summary>
    public static void NotNull<T>(T? obj, string message = "Value cannot be null.")
    {
        if (obj is null)
        {
            throw new ValidationException(message);
        }
    }

    /// <summary>
    /// Throws ValidationException if the string is null, empty, or whitespace.
    /// </summary>
    public static void NotNullOrEmpty(string? value, string message = "Value cannot be null or empty.")
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ValidationException(message);
        }
    }

    /// <summary>
    /// Throws NotFoundException if the requested entity was not found (is null).
    /// </summary>
    public static void Found<T>(T? entity, string message)
    {
        if (entity is null)
        {
            throw new NotFoundException(message);
        }
    }

    /// <summary>
    /// Throws NotFoundException if the requested entity was not found (is null).
    /// </summary>
    public static void Found<T>(T? entity, string entityName, object key)
    {
        if (entity is null)
        {
            throw new NotFoundException(entityName, key);
        }
    }

    /// <summary>
    /// Throws DomainException if a specific business rule is violated.
    /// </summary>
    public static void Against(bool ruleViolated, string message, string errorCode)
    {
        if (ruleViolated)
        {
            throw new DomainException(message, errorCode);
        }
    }

    /// <summary>
    /// Throws UnauthorizedException if the assertion is false.
    /// </summary>
    public static void Authorized(bool condition, string message = "Invalid username or password.")
    {
        if (!condition)
        {
            throw new UnauthorizedException(message);
        }
    }

    /// <summary>
    /// Throws ValidationException if any validation failures are present.
    /// </summary>
    public static void HasNoValidationFailures(IEnumerable<FluentValidation.Results.ValidationFailure> failures)
    {
        var failureList = failures as IList<FluentValidation.Results.ValidationFailure> ?? failures.ToList();
        if (failureList.Count > 0)
        {
            throw new ValidationException(failureList);
        }
    }
}
