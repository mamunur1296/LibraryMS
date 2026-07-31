using FluentValidation;
using LibraryMS.Application.Contracts.Branches;

namespace LibraryMS.Application.Contracts.Validators.Branch;

public sealed class UpdateBranchCommandValidator : AbstractValidator<UpdateBranchCommand>
{
    public UpdateBranchCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Branch ID is required.");
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Branch name is required.")
            .MaximumLength(200);
        RuleFor(x => x.Address).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Phone).NotEmpty().Matches(@"^[\d\s\+\-\(\)]{7,20}$");
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}
