using FluentValidation;
using LibraryMS.Application.Contracts.Members;

namespace LibraryMS.Application.Contracts.Validators.Member;

public sealed class DeleteMemberCommandValidator : AbstractValidator<DeleteMemberCommand>
{
    public DeleteMemberCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Member ID is required.");
    }
}
