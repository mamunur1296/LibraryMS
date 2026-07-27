using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Behaviours;

/// <summary>
/// Chain of Responsibility Link 1: Logging
/// MediatR pipeline behavior for request/response logging.
/// Logs slow queries (>500ms) as warnings.
/// </summary>
public sealed class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        => _logger = logger;

    public async Task<TResponse> Handle(
        TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        _logger.LogDebug("Handling {Request}", requestName);

        var sw = System.Diagnostics.Stopwatch.StartNew();
        var response = await next(cancellationToken);
        sw.Stop();

        if (sw.ElapsedMilliseconds > 500)
            _logger.LogWarning("Slow request {Request} took {Elapsed}ms", requestName, sw.ElapsedMilliseconds);
        else
            _logger.LogDebug("Handled {Request} in {Elapsed}ms", requestName, sw.ElapsedMilliseconds);

        return response;
    }
}

/// <summary>
/// Chain of Responsibility Link 2: Validation
/// MediatR pipeline behavior that auto-validates all commands/queries
/// using FluentValidation before they reach handlers.
/// Returns structured validation errors.
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        => _validators = validators;

    public async Task<TResponse> Handle(
        TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next(cancellationToken);

        var context = new ValidationContext<TRequest>(request);
        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count > 0)
            throw new Domain.Shared.Exceptions.ValidationException(failures);

        return await next(cancellationToken);
    }
}

/// <summary>
/// Marker interface: Commands/queries that implement this
/// will be automatically retried by RetryBehavior on failure.
/// </summary>
public interface IRetryableRequest { }

/// <summary>
/// Chain of Responsibility Link 3: Retry with Exponential Backoff
/// Automatically retries transient failures (e.g., DB deadlocks)
/// up to MaxRetries times. Each retry waits longer than the last.
/// </summary>
public sealed class RetryBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private const int MaxRetries = 3;
    private readonly ILogger<RetryBehavior<TRequest, TResponse>> _logger;

    public RetryBehavior(ILogger<RetryBehavior<TRequest, TResponse>> logger)
        => _logger = logger;

    public async Task<TResponse> Handle(
        TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Only retry requests that opt-in via IRetryableRequest marker
        if (request is not IRetryableRequest)
            return await next(cancellationToken);

        var requestName = typeof(TRequest).Name;
        var attempt = 0;

        while (true)
        {
            try
            {
                attempt++;
                return await next(cancellationToken);
            }
            catch (Exception ex) when (attempt < MaxRetries)
            {
                var delay = TimeSpan.FromMilliseconds(Math.Pow(2, attempt) * 100); // Exponential backoff: 200ms, 400ms, 800ms
                _logger.LogWarning(ex,
                    "Retryable request {Request} failed on attempt {Attempt}/{MaxRetries}. Retrying in {Delay}ms...",
                    requestName, attempt, MaxRetries, (int)delay.TotalMilliseconds);

                await Task.Delay(delay, cancellationToken);
            }
        }
    }
}
