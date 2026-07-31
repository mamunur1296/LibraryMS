using LibraryMS.Domain.Common;
using LibraryMS.EntityFrameworkCore.Outbox;

namespace LibraryMS.Infrastructure.Jobs.Handlers;

public interface IOutboxMessageHandler
{
    IOutboxMessageHandler SetNext(IOutboxMessageHandler handler);
    Task<bool> HandleAsync(OutboxMessage message, IDomainEvent? domainEvent, CancellationToken cancellationToken);
}
