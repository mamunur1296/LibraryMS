using FluentValidation;
using LibraryMS.Application.Contracts.Users;

namespace LibraryMS.Application.Contracts.Validators.Users;

public sealed class ChangeUserRoleCommandValidator : AbstractValidator<ChangeUserRoleCommand>
{
    public ChangeUserRoleCommandValidator()
    {
        RuleFor(x => x.TargetUserId)
            .NotEmpty().WithMessage("Target user ID is required.");

        RuleFor(x => x.NewRole)
            .NotEmpty().WithMessage("New role is required.")
            .Must(r => r is "Admin" or "Librarian" or "Member")
            .WithMessage("Role must be Admin, Librarian, or Member.");
    }
}
