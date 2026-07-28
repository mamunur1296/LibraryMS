using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Behaviours;

// Chain of Responsibility Link 1: Logging
// MediatR pipeline behavior for request/response logging.
// Logs slow queries (>500ms) as warnings.
public sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger) => _logger = logger;

    public async Task<TResponse> Handle( TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
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
