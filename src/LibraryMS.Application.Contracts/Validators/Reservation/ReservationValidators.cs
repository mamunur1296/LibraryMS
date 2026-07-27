using FluentValidation;
using LibraryMS.Application.Contracts.Reservations;

namespace LibraryMS.Application.Contracts.Validators.Reservation;

public sealed class CreateReservationCommandValidator : AbstractValidator<CreateReservationCommand>
{
    public CreateReservationCommandValidator()
    {
        RuleFor(x => x.MemberId).NotEmpty().WithMessage("Member ID is required.");
        RuleFor(x => x.BookId).NotEmpty().WithMessage("Book ID is required.");
        RuleFor(x => x.BranchId).NotEmpty().WithMessage("Branch ID is required.");
    }
}

public sealed class CancelReservationCommandValidator : AbstractValidator<CancelReservationCommand>
{
    public CancelReservationCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Reservation ID is required.");
        RuleFor(x => x.RequestingMemberId).NotEmpty();
    }
}
