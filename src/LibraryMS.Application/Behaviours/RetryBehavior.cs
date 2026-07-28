using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Behaviours;

// Marker interface: Commands/queries that implement this
// will be automatically retried by RetryBehavior on failure.
public interface IRetryableRequest { }

// Chain of Responsibility Link 3: Retry with Exponential Backoff
// Automatically retries transient failures (e.g., DB deadlocks)
// up to MaxRetries times. Each retry waits longer than the last.
public sealed class RetryBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private const int MaxRetries = 3;
    private readonly ILogger<RetryBehavior<TRequest, TResponse>> _logger;

    public RetryBehavior(ILogger<RetryBehavior<TRequest, TResponse>> logger) => _logger = logger;

    public async Task<TResponse> Handle( TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Only retry requests that opt-in via IRetryableRequest marker
        if (request is not IRetryableRequest) return await next(cancellationToken);

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
