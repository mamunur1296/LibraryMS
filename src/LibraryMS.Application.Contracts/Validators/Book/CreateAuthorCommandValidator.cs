using FluentValidation;
using LibraryMS.Application.Contracts.Books;

namespace LibraryMS.Application.Contracts.Validators.Book;

public sealed class CreateAuthorCommandValidator : AbstractValidator<CreateAuthorCommand>
{
    public CreateAuthorCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Author name is required.")
            .MaximumLength(200).WithMessage("Author name cannot exceed 200 characters.");

        RuleFor(x => x.Biography)
            .MaximumLength(2000).When(x => x.Biography is not null);
    }
}
