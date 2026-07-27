namespace LibraryMS.Domain.Shared;

/// <summary>
/// Unit of Work abstraction — wraps the DbContext SaveChanges.
/// Ensures domain events are dispatched after the transaction commits.
/// Defined here, implemented in EntityFrameworkCore layer.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
