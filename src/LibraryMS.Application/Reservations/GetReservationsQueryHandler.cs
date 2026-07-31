using LibraryMS.Application.Contracts.Common;
using LibraryMS.Application.Contracts.DTOs.Reservation;
using LibraryMS.Application.Contracts.Reservations;
using LibraryMS.Application.Mapping;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.BranchManagement;
using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.ReservationManagement;
using LibraryMS.Domain.ReservationManagement.AggregateRoots;
using LibraryMS.Domain.Shared.Guards;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Reservations;

public sealed class GetReservationsQueryHandler : IRequestHandler<GetReservationsQuery, PagedResult<ReservationDto>>
{
    private readonly IReservationRepository _repository;
    private readonly IMemberRepository _memberRepo;
    private readonly IBookRepository _bookRepo;
    private readonly IBranchRepository _branchRepo;
    private readonly ILogger<GetReservationsQueryHandler> _logger;

    public GetReservationsQueryHandler(
        IReservationRepository repository,
        IMemberRepository memberRepo,
        IBookRepository bookRepo,
        IBranchRepository branchRepo,
        ILogger<GetReservationsQueryHandler> logger)
    {
        _repository = repository;
        _memberRepo = memberRepo;
        _bookRepo = bookRepo;
        _branchRepo = branchRepo;
        _logger = logger;
    }

    public async Task<PagedResult<ReservationDto>> Handle(GetReservationsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving paged reservations. MemberId: {MemberId}, BookId: {BookId}, Status: {Status}, Page: {Page}, PageSize: {PageSize}",
            request.MemberId, request.BookId, request.Status, request.Page, request.PageSize);

        Ensure.Against(request.Page < 1, "Page number must be greater than or equal to 1.", "INVALID_PAGE");
        Ensure.Against(request.PageSize < 1, "Page size must be greater than or equal to 1.", "INVALID_PAGE_SIZE");

        var (items, total) = await _repository.GetPagedAsync(
            request.MemberId, request.BookId, request.Status,
            request.Page, request.PageSize, cancellationToken);

        var dtos = await HydrateNavigationProperties(items, cancellationToken);

        _logger.LogInformation("Successfully retrieved {Count} reservations out of {Total} total.", items.Count, total);

        return PagedResult<ReservationDto>.Create(dtos, total, request.Page, request.PageSize);
    }

    private async Task<List<ReservationDto>> HydrateNavigationProperties(List<Reservation> items, CancellationToken ct)
    {
        if (items.Count == 0) return [];

        var memberIds = items.Select(i => i.MemberId).Distinct().ToList();
        var bookIds = items.Select(i => i.BookId).Distinct().ToList();
        var branchIds = items.Select(i => i.BranchId).Distinct().ToList();

        var members = (await _memberRepo.GetByIdsAsync(memberIds, ct)).ToDictionary(m => m.Id);
        var books = (await _bookRepo.GetByIdsAsync(bookIds, ct)).ToDictionary(b => b.Id);
        var branches = (await _branchRepo.GetByIdsAsync(branchIds, ct)).ToDictionary(b => b.Id);

        return items.Select(reservation =>
            reservation.ToDto(
                members.GetValueOrDefault(reservation.MemberId),
                books.GetValueOrDefault(reservation.BookId),
                branches.GetValueOrDefault(reservation.BranchId)
            )
        ).ToList();
    }
}
