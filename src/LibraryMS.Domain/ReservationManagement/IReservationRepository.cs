namespace LibraryMS.Domain.ReservationManagement;

/// <summary>Repository contract for Reservation aggregate.</summary>
public interface IReservationRepository
{
    Task<Reservation?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<Reservation>> GetQueueForBookAsync(Guid bookId, Guid branchId, CancellationToken ct = default);
    Task<int> GetNextQueuePositionAsync(Guid bookId, Guid branchId, CancellationToken ct = default);
    Task<bool> HasActiveReservationAsync(Guid memberId, Guid bookId, CancellationToken ct = default);
    Task<List<Reservation>> GetExpiredReservationsAsync(CancellationToken ct = default);
    Task<(List<Reservation> Items, int TotalCount)> GetPagedAsync(
        Guid? memberId, Guid? bookId, string? status, int page, int pageSize, CancellationToken ct = default);
    Task AddAsync(Reservation reservation, CancellationToken ct = default);
    Task UpdateAsync(Reservation reservation, CancellationToken ct = default);
    Task UpdateRangeAsync(IEnumerable<Reservation> reservations, CancellationToken ct = default);
}
