using FluentValidation;
using LibraryMS.Application.Contracts.Books;

namespace LibraryMS.Application.Contracts.Validators.Book;

public sealed class UpdateBookCommandValidator : AbstractValidator<UpdateBookCommand>
{
    public UpdateBookCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(300);
        RuleFor(x => x.PublicationYear)
            .GreaterThanOrEqualTo(1000)
            .LessThanOrEqualTo(DateTime.UtcNow.Year + 1);
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.AuthorId).NotEmpty();
        RuleFor(x => x.Language).NotEmpty().MaximumLength(50);
    }
}
