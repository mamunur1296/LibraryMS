using LibraryMS.Application.Contracts.DTOs.Member;
using LibraryMS.Application.Contracts.Members;
using LibraryMS.Domain.BorrowManagement;
using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.ReservationManagement;
using LibraryMS.Domain.Shared.Enums;
using LibraryMS.Domain.Shared.Guards;
using MediatR;

namespace LibraryMS.Application.Members;

public sealed class GetMemberProfileStatsQueryHandler : IRequestHandler<GetMemberProfileStatsQuery, MemberProfileStatsDto>
{
    private readonly IMemberRepository _memberRepository;
    private readonly IBorrowRepository _borrowRepository;
    private readonly IReservationRepository _reservationRepository;

    public GetMemberProfileStatsQueryHandler(
        IMemberRepository memberRepository,
        IBorrowRepository borrowRepository,
        IReservationRepository reservationRepository)
    {
        _memberRepository = memberRepository;
        _borrowRepository = borrowRepository;
        _reservationRepository = reservationRepository;
    }

    public async Task<MemberProfileStatsDto> Handle(GetMemberProfileStatsQuery request, CancellationToken cancellationToken)
    {
        var member = await _memberRepository.GetByIdAsync(request.MemberId, cancellationToken);
        Ensure.Found(member, $"Member with ID '{request.MemberId}' was not found.");

        // Get total borrows (using paged query with pageSize = 1 just to get the TotalCount)
        var pagedBorrows = await _borrowRepository.GetPagedAsync(request.MemberId, null, null, 1, 1, cancellationToken);

        // Get active borrows (which includes overdue)
        var activeBorrows = await _borrowRepository.GetActiveBorrowsByMemberAsync(request.MemberId, cancellationToken);
        var activeCount = activeBorrows.Count;
        var overdueCount = activeBorrows.Count(b => b.Status == BorrowStatus.Overdue || b.IsOverdue);

        // Calculate fines (we need all borrows with unpaid fines for this member)
        // Since GetPagedAsync returns all, we might need to fetch all to calculate fines.
        // Wait, GetPagedAsync with int.MaxValue is safe for a single member since they won't have millions of borrows.
        var allBorrowsResult = await _borrowRepository.GetPagedAsync(request.MemberId, null, null, 1, 10000, cancellationToken);
        var totalFinesDue = allBorrowsResult.Items.Where(b => !b.IsFinePaid).Sum(b => b.LateFine);
        var totalFinesPaid = allBorrowsResult.Items.Where(b => b.IsFinePaid).Sum(b => b.LateFine);

        // Get active reservations
        // Let's just fetch all and filter, or use paged with max value.
        var allReservationsResult = await _reservationRepository.GetPagedAsync(request.MemberId, null, null, 1, 10000, cancellationToken);
        var activeReservations = allReservationsResult.Items.Count(r => r.Status == ReservationStatus.Pending || r.Status == ReservationStatus.Fulfilled);

        var nearestDueDate = activeBorrows.Any() ? activeBorrows.Min(b => b.DueDate) : (DateTime?)null;

        return new MemberProfileStatsDto
        {
            MemberId = request.MemberId,
            TotalBorrows = pagedBorrows.TotalCount,
            ActiveBorrows = activeCount,
            OverdueBorrows = overdueCount,
            ActiveReservations = activeReservations,
            TotalFinesDue = totalFinesDue,
            TotalFinesPaid = totalFinesPaid,
            MembershipExpiry = member.MembershipExpiry,
            NearestDueDate = nearestDueDate,
            FavouriteCount = member.Favorites.Count
        };
    }
}
