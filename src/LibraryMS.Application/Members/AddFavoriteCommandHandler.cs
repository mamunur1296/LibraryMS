using LibraryMS.Application.Contracts.Members;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.Shared;
using LibraryMS.Domain.Shared.Guards;
using MediatR;

namespace LibraryMS.Application.Members;

internal sealed class AddFavoriteCommandHandler : IRequestHandler<AddFavoriteCommand>
{
    private readonly IMemberRepository _memberRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddFavoriteCommandHandler(IMemberRepository memberRepository, IBookRepository bookRepository, IUnitOfWork unitOfWork)
    {
        _memberRepository = memberRepository;
        _bookRepository = bookRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(AddFavoriteCommand request, CancellationToken cancellationToken)
    {
        var member = await _memberRepository.GetByIdAsync(request.MemberId, cancellationToken);
        Ensure.Found(member, "Member not found.");

        var book = await _bookRepository.GetByIdAsync(request.BookId, cancellationToken);
        Ensure.Found(book, "Book not found.");

        member.AddFavorite(request.BookId);

        await _memberRepository.UpdateAsync(member, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
