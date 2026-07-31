using LibraryMS.Domain.Common;
using LibraryMS.EntityFrameworkCore.Outbox;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Infrastructure.Jobs.Handlers;

public sealed class DomainEventOutboxMessageHandler : AbstractOutboxMessageHandler
{
    private readonly IPublisher _publisher;
    private readonly ILogger<DomainEventOutboxMessageHandler> _logger;

    public DomainEventOutboxMessageHandler(
        IPublisher publisher,
        ILogger<DomainEventOutboxMessageHandler> logger)
    {
        _publisher = publisher;
        _logger = logger;
    }

    public override async Task<bool> HandleAsync(OutboxMessage message, IDomainEvent? domainEvent, CancellationToken cancellationToken)
    {
        // For messages that have no Category or have a standard DomainEvent category, we publish them via MediatR
        if (string.IsNullOrEmpty(message.Category) || message.Category == "DomainEvent")
        {
            if (domainEvent != null)
            {
                await _publisher.Publish(domainEvent, cancellationToken);
                return true; // Handled
            }
            else
            {
                _logger.LogWarning("DomainEventOutboxMessageHandler received a message with null domain event: {MessageId}", message.Id);
                // Even though it's null, we might consider it handled to avoid getting stuck if it shouldn't be processed by the next handler.
                // But typically it means deserialization failed. Let it fall through or fail in the caller.
            }
        }

        return await base.HandleAsync(message, domainEvent, cancellationToken);
    }
}
