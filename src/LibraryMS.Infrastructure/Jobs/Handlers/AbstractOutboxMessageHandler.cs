using LibraryMS.Domain.Common;
using LibraryMS.EntityFrameworkCore.Outbox;

namespace LibraryMS.Infrastructure.Jobs.Handlers;

public abstract class AbstractOutboxMessageHandler : IOutboxMessageHandler
{
    private IOutboxMessageHandler? _nextHandler;

    public IOutboxMessageHandler SetNext(IOutboxMessageHandler handler)
    {
        _nextHandler = handler;
        return handler;
    }

    public virtual async Task<bool> HandleAsync(OutboxMessage message, IDomainEvent? domainEvent, CancellationToken cancellationToken)
    {
        if (_nextHandler != null)
        {
            return await _nextHandler.HandleAsync(message, domainEvent, cancellationToken);
        }
        return false; // Not handled
    }
}
