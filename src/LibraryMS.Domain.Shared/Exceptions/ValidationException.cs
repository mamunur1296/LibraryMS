using FluentValidation.Results;

namespace LibraryMS.Domain.Shared.Exceptions;

/// <summary>
/// Thrown when FluentValidation detects invalid input.
/// Returns HTTP 400 with structured error details.
/// </summary>
public sealed class ValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException()
        : base("One or more validation failures have occurred.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        Errors = failures
            .GroupBy(f => f.PropertyName, f => f.ErrorMessage)
            .ToDictionary(
                g => g.Key,
                g => g.ToArray()
            );
    }
}
