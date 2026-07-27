using FluentValidation;
using LibraryMS.Application.Contracts.Borrows;
using LibraryMS.Domain.BorrowManagement;

namespace LibraryMS.Application.Contracts.Validators.Borrow;

public sealed class BorrowBookCommandValidator : AbstractValidator<BorrowBookCommand>
{
    public BorrowBookCommandValidator()
    {
        RuleFor(x => x.MemberId).NotEmpty().WithMessage("Member ID is required.");
        RuleFor(x => x.BookCopyId).NotEmpty().WithMessage("Book copy ID is required.");
        RuleFor(x => x.BookId).NotEmpty().WithMessage("Book ID is required.");
        RuleFor(x => x.BranchId).NotEmpty().WithMessage("Branch ID is required.");

        RuleFor(x => x.BorrowDays)
            .GreaterThan(0).WithMessage("Borrow days must be greater than 0.")
            .LessThanOrEqualTo(30).WithMessage("Borrow duration cannot exceed 30 days.")
            .When(x => x.BorrowDays.HasValue);
    }
}

public sealed class ReturnBookCommandValidator : AbstractValidator<ReturnBookCommand>
{
    public ReturnBookCommandValidator()
    {
        RuleFor(x => x.BorrowId).NotEmpty().WithMessage("Borrow ID is required.");
        RuleFor(x => x.Notes).MaximumLength(500).When(x => x.Notes is not null);
    }
}
