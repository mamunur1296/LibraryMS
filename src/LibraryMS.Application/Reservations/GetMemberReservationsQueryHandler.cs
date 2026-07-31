using LibraryMS.Application.Contracts.DTOs.Reservation;
using LibraryMS.Application.Contracts.Reservations;
using LibraryMS.Application.Mapping;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.BranchManagement;
using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.ReservationManagement;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Reservations;

public sealed class GetMemberReservationsQueryHandler : IRequestHandler<GetMemberReservationsQuery, List<ReservationDto>>
{
    private readonly IReservationRepository _reservationRepo;
    private readonly IMemberRepository _memberRepo;
    private readonly IBookRepository _bookRepo;
    private readonly IBranchRepository _branchRepo;
    private readonly ILogger<GetMemberReservationsQueryHandler> _logger;

    public GetMemberReservationsQueryHandler(
        IReservationRepository reservationRepo,
        IMemberRepository memberRepo,
        IBookRepository bookRepo,
        IBranchRepository branchRepo,
        ILogger<GetMemberReservationsQueryHandler> logger)
    {
        _reservationRepo = reservationRepo;
        _memberRepo = memberRepo;
        _bookRepo = bookRepo;
        _branchRepo = branchRepo;
        _logger = logger;
    }

    public async Task<List<ReservationDto>> Handle(GetMemberReservationsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving reservations list for Member: {MemberId}", request.MemberId);

        var (reservations, _) = await _reservationRepo.GetPagedAsync(
            request.MemberId, null, null, 1, int.MaxValue, cancellationToken);

        var bookIds = reservations.Select(r => r.BookId).Distinct().ToList();
        var branchIds = reservations.Select(r => r.BranchId).Distinct().ToList();

        var member = await _memberRepo.GetByIdAsync(request.MemberId, cancellationToken);
        var books = (await _bookRepo.GetByIdsAsync(bookIds, cancellationToken)).ToDictionary(b => b.Id);
        var branches = (await _branchRepo.GetByIdsAsync(branchIds, cancellationToken)).ToDictionary(b => b.Id);

        return reservations.Select(r =>
            r.ToDto(member, books.GetValueOrDefault(r.BookId), branches.GetValueOrDefault(r.BranchId))
        ).ToList();
    }
}
