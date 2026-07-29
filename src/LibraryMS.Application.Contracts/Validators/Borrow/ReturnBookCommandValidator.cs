using FluentValidation;
using LibraryMS.Application.Contracts.Borrows;

namespace LibraryMS.Application.Contracts.Validators.Borrow;

public sealed class ReturnBookCommandValidator : AbstractValidator<ReturnBookCommand>
{
    public ReturnBookCommandValidator()
    {
        RuleFor(x => x.BorrowId).NotEmpty().WithMessage("Borrow ID is required.");
        RuleFor(x => x.Notes).MaximumLength(500).When(x => x.Notes is not null);
    }
}
