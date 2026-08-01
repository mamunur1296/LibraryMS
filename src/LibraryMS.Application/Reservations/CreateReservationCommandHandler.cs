using LibraryMS.Application.Contracts.DTOs.Reservation;
using LibraryMS.Application.Contracts.Reservations;
using LibraryMS.Application.Mapping;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.BookManagement.AggregateRoots;
using LibraryMS.Domain.BookManagement.Entities;
using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.MemberManagement.AggregateRoots;
using LibraryMS.Domain.MemberManagement.Services;
using LibraryMS.Domain.ReservationManagement;
using LibraryMS.Domain.ReservationManagement.AggregateRoots;
using LibraryMS.Domain.ReservationManagement.Services;
using LibraryMS.Domain.BorrowManagement;
using LibraryMS.Domain.Shared;
using LibraryMS.Domain.Shared.Guards;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Reservations;

public sealed class CreateReservationCommandHandler : IRequestHandler<CreateReservationCommand, ReservationDto>
{
    private readonly IReservationRepository _reservationRepo;
    private readonly IBookRepository _bookRepo;
    private readonly IMemberRepository _memberRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBorrowRepository _borrowRepo;
    private readonly ILogger<CreateReservationCommandHandler> _logger;
    private readonly ReservationManager _reservationManager;

    public CreateReservationCommandHandler(
        IReservationRepository reservationRepo,
        IBookRepository bookRepo,
        IMemberRepository memberRepo,
        IBorrowRepository borrowRepo,
        IUnitOfWork unitOfWork,
        ILogger<CreateReservationCommandHandler> logger,
        ReservationManager reservationManager)
    {
        _reservationRepo = reservationRepo;
        _bookRepo = bookRepo; 
        _memberRepo = memberRepo;
        _borrowRepo = borrowRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _reservationManager = reservationManager;
    }

    public async Task<ReservationDto> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing CreateReservationCommand for MemberId: {MemberId}, BookId: {BookId}, BranchId: {BranchId}",
            request.MemberId, request.BookId, request.BranchId);

        // Validate member exists and is active
        var member = await _memberRepo.GetByIdAsync(request.MemberId, cancellationToken);
        Ensure.Found(member, $"Member with ID '{request.MemberId}' was not found.");

        Ensure.Against(!member!.CanBorrow(), "Suspended members cannot create reservations.", "RESERVATION_SUSPENDED_MEMBER");

        var hasUnpaidFine = await _borrowRepo.HasUnpaidFineAsync(request.MemberId, cancellationToken);
        Ensure.Against(hasUnpaidFine, "Member has unpaid fines and cannot reserve books.", "RESERVATION_MEMBER_HAS_FINE");

        // Validate book exists
        var book = await _bookRepo.GetByIdWithCopiesAsync(request.BookId, cancellationToken);
        Ensure.Found(book, $"Book with ID '{request.BookId}' was not found.");

        // Cannot reserve if copies are available
        var availableCopy = book!.GetAvailableCopyInBranch(request.BranchId);
        Ensure.Against(availableCopy is not null, "A copy is currently available. Please borrow directly instead of reserving.", "RESERVATION_COPY_AVAILABLE");

        // No duplicate reservations
        var hasExisting = await _reservationRepo.HasActiveReservationAsync(
            request.MemberId, request.BookId, cancellationToken);
        Ensure.Against(hasExisting, "Member already has an active reservation for this book.", "RESERVATION_DUPLICATE");

        var queuePosition = await _reservationRepo.GetNextQueuePositionAsync(
            request.BookId, request.BranchId, cancellationToken);

        var reservation = _reservationManager.Create(
            request.MemberId, request.BookId,
            request.BranchId, queuePosition);

        var dbFailed = false;
        try
        {
            await _reservationRepo.AddAsync(reservation, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save reservation for member {MemberId} and book {BookId}.", request.MemberId, request.BookId);
            dbFailed = true;
        }

        Ensure.Against(dbFailed, "An error occurred while creating the reservation in the database.", "DB_UPDATE_ERROR");

        _logger.LogInformation("Reservation created for member {MemberId}, book {BookId}, position #{Position}",
            request.MemberId, request.BookId, queuePosition);

        return reservation.ToDto();
    }
}

