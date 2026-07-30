using FluentValidation;
using LibraryMS.Application.Contracts.Branches;

namespace LibraryMS.Application.Contracts.Validators.Branch;

public sealed class DeactivateBranchCommandValidator : AbstractValidator<DeactivateBranchCommand>
{
    public DeactivateBranchCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Branch ID is required.");
    }
}
