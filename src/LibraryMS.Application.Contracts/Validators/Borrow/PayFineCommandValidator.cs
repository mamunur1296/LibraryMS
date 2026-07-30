using FluentValidation;
using LibraryMS.Application.Contracts.Borrows;

namespace LibraryMS.Application.Contracts.Validators.Borrow;

public sealed class PayFineCommandValidator : AbstractValidator<PayFineCommand>
{
    public PayFineCommandValidator()
    {
        RuleFor(x => x.BorrowId)
            .NotEmpty().WithMessage("Borrow ID is required.");
    }
}
