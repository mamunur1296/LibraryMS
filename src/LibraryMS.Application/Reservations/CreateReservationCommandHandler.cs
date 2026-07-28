using LibraryMS.Domain.Shared;
using LibraryMS.Application.Mapping;
using LibraryMS.Application.Contracts.Reservations;
using LibraryMS.Application.Contracts.DTOs.Reservation;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.ReservationManagement;
using LibraryMS.Domain.Shared.Exceptions;
using LibraryMS.Domain.Shared.Guards;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryMS.Application.Reservations;

public sealed class CreateReservationCommandHandler : IRequestHandler<CreateReservationCommand, ReservationDto>
{
    private readonly IReservationRepository _reservationRepo;
    private readonly IBookRepository _bookRepo;
    private readonly IMemberRepository _memberRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateReservationCommandHandler> _logger;

    public CreateReservationCommandHandler(
        IReservationRepository reservationRepo,
        IBookRepository bookRepo,
        IMemberRepository memberRepo,
        IUnitOfWork unitOfWork,
        ILogger<CreateReservationCommandHandler> logger)
    {
        _reservationRepo = reservationRepo;
        _bookRepo = bookRepo; _memberRepo = memberRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ReservationDto> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
    {
        // Validate member exists and is active
        var member = await _memberRepo.GetByIdAsync(request.MemberId, cancellationToken);
        Ensure.Found(member, $"Member with ID '{request.MemberId}' was not found.");

        Ensure.Against(!member!.CanBorrow(), "Suspended members cannot create reservations.", "RESERVATION_SUSPENDED_MEMBER");

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

        var reservation = new Reservation(
            Guid.NewGuid(), request.MemberId, request.BookId,
            request.BranchId, queuePosition);

        await _reservationRepo.AddAsync(reservation, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Reservation created for member {MemberId}, book {BookId}, position #{Position}",
            request.MemberId, request.BookId, queuePosition);

        return reservation.ToDto();
    }
}
