using FluentValidation;
using LibraryMS.Application.Contracts.Books;

namespace LibraryMS.Application.Contracts.Validators.Book;

public sealed class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
{
    public CreateBookCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Book title is required.")
            .MaximumLength(300).WithMessage("Title cannot exceed 300 characters.");

        RuleFor(x => x.ISBN)
            .NotEmpty().WithMessage("ISBN is required.")
            .Must(BeValidISBN).WithMessage("ISBN must be a valid 10 or 13 digit number (hyphens allowed).");

        RuleFor(x => x.PublicationYear)
            .GreaterThanOrEqualTo(1000).WithMessage("Publication year must be at least 1000.")
            .LessThanOrEqualTo(DateTime.UtcNow.Year + 1)
                .WithMessage($"Publication year cannot exceed {DateTime.UtcNow.Year + 1}.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category is required.");

        RuleFor(x => x.AuthorId)
            .NotEmpty().WithMessage("Author is required.");

        RuleFor(x => x.Language)
            .NotEmpty().WithMessage("Language is required.")
            .MaximumLength(50);

        RuleFor(x => x.InitialCopies)
            .GreaterThan(0).WithMessage("At least 1 copy is required.")
            .LessThanOrEqualTo(100).WithMessage("Cannot add more than 100 copies at once.");

        RuleFor(x => x.BranchId)
            .NotEmpty().WithMessage("Branch is required for initial copies.");
    }

    private static bool BeValidISBN(string isbn)
    {
        var cleaned = isbn?.Replace("-", "").Replace(" ", "") ?? string.Empty;
        return (cleaned.Length == 10 || cleaned.Length == 13) && cleaned.All(char.IsDigit);
    }
}
