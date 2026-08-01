using FluentValidation;
using LibraryMS.Application.Contracts.Users;

namespace LibraryMS.Application.Contracts.Validators.Users;

public sealed class ChangeUsernameCommandValidator : AbstractValidator<ChangeUsernameCommand>
{
    public ChangeUsernameCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.NewUsername)
            .NotEmpty().WithMessage("New username is required.")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters.")
            .MaximumLength(50).WithMessage("Username cannot exceed 50 characters.");
    }
}
