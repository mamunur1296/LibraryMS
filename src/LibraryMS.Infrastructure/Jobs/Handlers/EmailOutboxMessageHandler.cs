using LibraryMS.Domain.Common;
using LibraryMS.EntityFrameworkCore.Outbox;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Infrastructure.Jobs.Handlers;

public sealed class EmailOutboxMessageHandler : AbstractOutboxMessageHandler
{
    private readonly ILogger<EmailOutboxMessageHandler> _logger;

    public EmailOutboxMessageHandler(ILogger<EmailOutboxMessageHandler> logger)
    {
        _logger = logger;
    }

    public override async Task<bool> HandleAsync(OutboxMessage message, IDomainEvent? domainEvent, CancellationToken cancellationToken)
    {
        if (message.Category == "Email")
        {
            _logger.LogInformation("Processing Email outbox message: {Id}", message.Id);
            // Example: send email using IEmailService with payload from message.Content
            
            // For now just simulate successful handling
            return true; 
        }

        return await base.HandleAsync(message, domainEvent, cancellationToken);
    }
}
