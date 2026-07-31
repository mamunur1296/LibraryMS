using FluentValidation;
using LibraryMS.Application.Contracts.Books;

namespace LibraryMS.Application.Contracts.Validators.Book;

public sealed class AddBookCopiesCommandValidator : AbstractValidator<AddBookCopiesCommand>
{
    public AddBookCopiesCommandValidator()
    {
        RuleFor(x => x.BookId).NotEmpty();
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be at least 1.")
            .LessThanOrEqualTo(50).WithMessage("Cannot add more than 50 copies at once.");
    }
}
