using LibraryMS.Application.Mapping;
using LibraryMS.Application.Contracts.Members;
using LibraryMS.Application.Contracts.DTOs.Member;
using LibraryMS.Application.Contracts.Common;
using LibraryMS.Domain.MemberManagement;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryMS.Application.Members;

public sealed class SearchMembersQueryHandler : IRequestHandler<SearchMembersQuery, PagedResult<MemberDto>>
{
    private readonly IMemberRepository _repository;

    public SearchMembersQueryHandler(IMemberRepository repository)
    {
        _repository = repository; 
    }

    public async Task<PagedResult<MemberDto>> Handle(SearchMembersQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await _repository.SearchAsync(
            request.SearchTerm, request.Status,
            request.Page, request.PageSize, cancellationToken);

        return PagedResult<MemberDto>.Create(
            items.Select(i => i.ToDto()).ToList(),
            total, request.Page, request.PageSize);
    }
}
