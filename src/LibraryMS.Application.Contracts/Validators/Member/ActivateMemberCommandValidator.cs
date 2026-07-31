using FluentValidation;
using LibraryMS.Application.Contracts.Members;

namespace LibraryMS.Application.Contracts.Validators.Member;

public sealed class ActivateMemberCommandValidator : AbstractValidator<ActivateMemberCommand>
{
    public ActivateMemberCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Member ID is required.");
    }
}
