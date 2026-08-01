using System;
using LibraryMS.Domain.Common;
using LibraryMS.Domain.ReservationManagement.AggregateRoots;

namespace LibraryMS.Domain.ReservationManagement.Services;

public sealed class ReservationManager
{
    private readonly IGuidGenerator _guidGenerator;

    public ReservationManager(IGuidGenerator guidGenerator)
    {
        _guidGenerator = guidGenerator;
    }

    public Reservation Create(Guid memberId, Guid bookId, Guid branchId, int queuePosition)
    {
        return new Reservation(_guidGenerator.Create(), memberId, bookId, branchId, queuePosition);
    }
}
