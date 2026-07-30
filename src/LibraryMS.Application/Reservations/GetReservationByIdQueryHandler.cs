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

public sealed class GetReservationByIdQueryHandler : IRequestHandler<GetReservationByIdQuery, ReservationDto?>
{
    private readonly IReservationRepository _repository;
    private readonly IMemberRepository _memberRepo;
    private readonly IBookRepository _bookRepo;
    private readonly IBranchRepository _branchRepo;
    private readonly ILogger<GetReservationByIdQueryHandler> _logger;

    public GetReservationByIdQueryHandler(
        IReservationRepository repository,
        IMemberRepository memberRepo,
        IBookRepository bookRepo,
        IBranchRepository branchRepo,
        ILogger<GetReservationByIdQueryHandler> logger)
    {
        _repository = repository;
        _memberRepo = memberRepo;
        _bookRepo = bookRepo;
        _branchRepo = branchRepo;
        _logger = logger;
    }

    public async Task<ReservationDto?> Handle(GetReservationByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving reservation with ID: {ReservationId}", request.Id);

        var reservation = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (reservation is null)
        {
            _logger.LogWarning("Reservation with ID '{ReservationId}' was not found.", request.Id);
            return null;
        }

        var member = await _memberRepo.GetByIdAsync(reservation.MemberId, cancellationToken);
        var book = await _bookRepo.GetByIdAsync(reservation.BookId, cancellationToken);
        var branch = await _branchRepo.GetByIdAsync(reservation.BranchId, cancellationToken);

        _logger.LogInformation("Successfully retrieved reservation with ID: {ReservationId}", request.Id);

        return reservation.ToDto(member, book, branch);
    }
}
