using FluentValidation;
using LibraryMS.Application.Contracts.Users;

namespace LibraryMS.Application.Users;

public class CreateLibrarianCommandValidator : AbstractValidator<CreateLibrarianCommand>
{
    public CreateLibrarianCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters.");
    }
}

public class AssignLibrarianToBranchCommandValidator : AbstractValidator<AssignLibrarianToBranchCommand>
{
    public AssignLibrarianToBranchCommandValidator()
    {
        RuleFor(x => x.LibrarianId).NotEmpty().WithMessage("LibrarianId is required.");
        RuleFor(x => x.BranchId).NotEmpty().WithMessage("BranchId is required.");
    }
}


public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required.");
        RuleFor(x => x.CurrentPassword).NotEmpty().WithMessage("Current password is required.");
        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required.")
            .MinimumLength(6).WithMessage("New password must be at least 6 characters.")
            .NotEqual(x => x.CurrentPassword).WithMessage("New password must be different from current password.");
    }
}
