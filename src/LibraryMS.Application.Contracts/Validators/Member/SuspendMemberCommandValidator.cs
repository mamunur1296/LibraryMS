using FluentValidation;
using LibraryMS.Application.Contracts.Members;

namespace LibraryMS.Application.Contracts.Validators.Member;

public sealed class SuspendMemberCommandValidator : AbstractValidator<SuspendMemberCommand>
{
    public SuspendMemberCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.SuspendedUntil)
            .GreaterThan(DateTime.UtcNow).WithMessage("Suspension end date must be in the future.");
        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Suspension reason is required.")
            .MaximumLength(500);
    }
}
