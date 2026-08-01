using LibraryMS.Application.Contracts.DTOs.Member;
using LibraryMS.Application.Contracts.Members;
using LibraryMS.Application.Mapping;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.Shared.Guards;
using MediatR;

namespace LibraryMS.Application.Members;

internal sealed class GetMemberFavoritesQueryHandler : IRequestHandler<GetMemberFavoritesQuery, List<MemberFavoriteDto>>
{
    private readonly IMemberRepository _memberRepository;
    private readonly IBookRepository _bookRepository;

    public GetMemberFavoritesQueryHandler(IMemberRepository memberRepository, IBookRepository bookRepository)
    {
        _memberRepository = memberRepository;
        _bookRepository = bookRepository;
    }

    public async Task<List<MemberFavoriteDto>> Handle(GetMemberFavoritesQuery request, CancellationToken cancellationToken)
    {
        var member = await _memberRepository.GetByIdAsync(request.MemberId, cancellationToken);
        Ensure.Found(member, "Member not found.");

        var bookIds = member.Favorites.Select(f => f.BookId).ToList();
        if (!bookIds.Any())
        {
            return new List<MemberFavoriteDto>();
        }

        var books = await _bookRepository.GetByIdsAsync(bookIds, cancellationToken);

        return books.Select(b => new MemberFavoriteDto
        {
            BookId = b.Id,
            Book = b.ToDto()
        }).ToList();
    }
}
