using LibraryMS.Application.Mapping;
using LibraryMS.Application.Contracts.Reservations;
using LibraryMS.Application.Contracts.Common;
using LibraryMS.Application.Contracts.DTOs.Reservation;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.ReservationManagement;
using LibraryMS.Domain.Shared.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

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
        var member = await _memberRepo.GetByIdAsync(request.MemberId, cancellationToken)
            ?? throw new NotFoundException(nameof(Member), request.MemberId);

        if (!member.CanBorrow())
            throw new DomainException("Suspended members cannot create reservations.", "RESERVATION_SUSPENDED_MEMBER");

        // Validate book exists
        var book = await _bookRepo.GetByIdWithCopiesAsync(request.BookId, cancellationToken)
            ?? throw new NotFoundException(nameof(Book), request.BookId);

        // Cannot reserve if copies are available
        var availableCopy = book.GetAvailableCopyInBranch(request.BranchId);
        if (availableCopy is not null)
            throw new DomainException(
                "A copy is currently available. Please borrow directly instead of reserving.",
                "RESERVATION_COPY_AVAILABLE");

        // No duplicate reservations
        var hasExisting = await _reservationRepo.HasActiveReservationAsync(
            request.MemberId, request.BookId, cancellationToken);
        if (hasExisting)
            throw new DomainException("Member already has an active reservation for this book.", "RESERVATION_DUPLICATE");

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

public sealed class CancelReservationCommandHandler : IRequestHandler<CancelReservationCommand>
{
    private readonly IReservationRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelReservationCommandHandler(IReservationRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository; _unitOfWork = unitOfWork;
    }

    public async Task Handle(CancelReservationCommand request, CancellationToken cancellationToken)
    {
        var reservation = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Reservation), request.Id);

        // Members can only cancel their own reservations
        if (reservation.MemberId != request.RequestingMemberId)
            throw new ForbiddenException("You can only cancel your own reservations.");

        var cancelMethod = typeof(Reservation).GetMethod("Cancel",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        cancelMethod!.Invoke(reservation, null);

        // Shift queue positions for remaining reservations
        var queue = await _repository.GetQueueForBookAsync(
            reservation.BookId, reservation.BranchId, cancellationToken);

        var subsequent = queue
            .Where(r => r.QueuePosition > reservation.QueuePosition)
            .OrderBy(r => r.QueuePosition)
            .ToList();

        foreach (var r in subsequent)
        {
            var updatePos = typeof(Reservation).GetMethod("UpdateQueuePosition",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            updatePos!.Invoke(r, [r.QueuePosition - 1]);
        }

        await _repository.UpdateAsync(reservation, cancellationToken);
        if (subsequent.Count > 0)
            await _repository.UpdateRangeAsync(subsequent, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

public sealed class GetReservationsQueryHandler : IRequestHandler<GetReservationsQuery, PagedResult<ReservationDto>>
{
    private readonly IReservationRepository _repository;
    

    public GetReservationsQueryHandler(IReservationRepository repository)
    {
        _repository = repository; 
    }

    public async Task<PagedResult<ReservationDto>> Handle(GetReservationsQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await _repository.GetPagedAsync(
            request.MemberId, request.BookId, request.Status,
            request.Page, request.PageSize, cancellationToken);

        return PagedResult<ReservationDto>.Create(
            items.Select(i => i.ToDto()).ToList(),
            total, request.Page, request.PageSize);
    }
}
