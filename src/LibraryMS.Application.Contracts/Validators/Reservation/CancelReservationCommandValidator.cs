using FluentValidation;
using LibraryMS.Application.Contracts.Reservations;

namespace LibraryMS.Application.Contracts.Validators.Reservation;

public sealed class CancelReservationCommandValidator : AbstractValidator<CancelReservationCommand>
{
    public CancelReservationCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Reservation ID is required.");
        RuleFor(x => x.RequestingMemberId).NotEmpty();
    }
}
