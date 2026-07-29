using LibraryMS.Application.Contracts.Common;
using LibraryMS.Application.Contracts.DTOs.Member;
using LibraryMS.Application.Contracts.Members;
using LibraryMS.Application.Mapping;
using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.Shared.Guards;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Members;

public sealed class SearchMembersQueryHandler : IRequestHandler<SearchMembersQuery, PagedResult<MemberDto>>
{
    private readonly IMemberRepository _repository;
    private readonly ILogger<SearchMembersQueryHandler> _logger;

    public SearchMembersQueryHandler(IMemberRepository repository, ILogger<SearchMembersQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<PagedResult<MemberDto>> Handle(SearchMembersQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Searching members with Term: '{SearchTerm}', Status: '{Status}', Page: {Page}, PageSize: {PageSize}",
            request.SearchTerm, request.Status, request.Page, request.PageSize);

        Ensure.Against(request.Page < 1, "Page number must be greater than or equal to 1.", "INVALID_PAGE");
        Ensure.Against(request.PageSize < 1, "Page size must be greater than or equal to 1.", "INVALID_PAGE_SIZE");

        var (items, total) = await _repository.SearchAsync(
            request.SearchTerm, request.Status,
            request.Page, request.PageSize, cancellationToken);

        _logger.LogInformation("Successfully found {Count} members out of {Total} total.", items.Count, total);

        return PagedResult<MemberDto>.Create(
            items.Select(i => i.ToDto()).ToList(),
            total, request.Page, request.PageSize);
    }
}
