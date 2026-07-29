using LibraryMS.Application.Contracts.DTOs.Reservation;
using LibraryMS.Application.Contracts.Reservations;
using LibraryMS.Application.Mapping;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.BranchManagement;
using LibraryMS.Domain.ReservationManagement;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryMS.Application.Reservations;

public sealed class GetBookQueueQueryHandler : IRequestHandler<GetBookQueueQuery, ReservationQueueDto>
{
    private readonly IReservationRepository _reservationRepo;
    private readonly IBookRepository _bookRepo;
    private readonly IBranchRepository _branchRepo;
    private readonly ILogger<GetBookQueueQueryHandler> _logger;

    public GetBookQueueQueryHandler(
        IReservationRepository reservationRepo,
        IBookRepository bookRepo,
        IBranchRepository branchRepo,
        ILogger<GetBookQueueQueryHandler> logger)
    {
        _reservationRepo = reservationRepo;
        _bookRepo = bookRepo;
        _branchRepo = branchRepo;
        _logger = logger;
    }

    public async Task<ReservationQueueDto> Handle(GetBookQueueQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving reservation queue for Book: {BookId} and Branch: {BranchId}", request.BookId, request.BranchId);

        var queueReservations = await _reservationRepo.GetQueueForBookAsync(request.BookId, request.BranchId, cancellationToken);
        var book = await _bookRepo.GetByIdAsync(request.BookId, cancellationToken);
        var branches = await _branchRepo.GetAllAsync(cancellationToken);
        var branch = branches.FirstOrDefault(b => b.Id == request.BranchId);

        var queueList = queueReservations.Select(r => r.ToDto()).ToList();

        return new ReservationQueueDto
        {
            BookId = request.BookId,
            BookTitle = book?.Title ?? "Unknown Book",
            BranchId = request.BranchId,
            BranchName = branch?.Name ?? "Unknown Branch",
            TotalInQueue = queueList.Count,
            Queue = queueList
        };
    }
}
