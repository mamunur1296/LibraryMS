using FluentValidation.Results;

namespace LibraryMS.Domain.Shared.Exceptions;

// Thrown when FluentValidation detects invalid input.
// Returns HTTP 400 with structured error details.
public sealed class ValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException()
        : base("One or more validation failures have occurred.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(string message)
        : base(message)
    {
        Errors = new Dictionary<string, string[]>
        {
            { string.Empty, new[] { message } }
        };
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
