using FluentValidation;
using LibraryMS.Domain.Shared.Guards;
using MediatR;

namespace LibraryMS.Application.Behaviours;

// Chain of Responsibility Link 2: Validation
// MediatR pipeline behavior that auto-validates all commands/queries
// using FluentValidation before they reach handlers.
// Returns structured validation errors.
public sealed class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any()) return await next(cancellationToken);

        var context = new ValidationContext<TRequest>(request);
        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        Ensure.HasNoValidationFailures(failures);

        return await next(cancellationToken);
    }
}
