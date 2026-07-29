using FluentValidation;
using LibraryMS.Application.Contracts.Members;

namespace LibraryMS.Application.Contracts.Validators.Member;

public sealed class UpdateMemberCommandValidator : AbstractValidator<UpdateMemberCommand>
{
    public UpdateMemberCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Phone).NotEmpty().Matches(@"^[\d\s\+\-\(\)]{7,20}$");
        RuleFor(x => x.Address).MaximumLength(500).When(x => x.Address is not null);
    }
}
