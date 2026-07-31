using LibraryMS.Application.Contracts.Common;
using LibraryMS.Application.Contracts.DTOs.Member;
using LibraryMS.Application.Contracts.Members;
using LibraryMS.Application.Mapping;
using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.IdentityManagement;
using LibraryMS.Domain.Shared.Guards;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Members;

public sealed class SearchMembersQueryHandler : IRequestHandler<SearchMembersQuery, PagedResult<MemberDto>>
{
    private readonly IMemberRepository _repository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<SearchMembersQueryHandler> _logger;

    public SearchMembersQueryHandler(
        IMemberRepository repository,
        IUserRepository userRepository,
        ILogger<SearchMembersQueryHandler> logger)
    {
        _repository = repository;
        _userRepository = userRepository;
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

        var activeBorrowCounts = new List<int>();
        var hasAccountList = new List<bool>();
        foreach (var item in items)
        {
            var count = await _repository.GetActiveBorrowCountAsync(item.Id, cancellationToken);
            activeBorrowCounts.Add(count);

            var user = await _userRepository.GetByMemberIdAsync(item.Id, cancellationToken);
            hasAccountList.Add(user != null);
        }

        var dtos = items.Select((member, i) =>
        {
            var dto = member.ToDto();
            dto = new MemberDto
            {
                Id = dto.Id,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone,
                MembershipNumber = dto.MembershipNumber,
                Address = dto.Address,
                Status = dto.Status,
                JoinDate = dto.JoinDate,
                SuspendedUntil = dto.SuspendedUntil,
                ActiveBorrows = activeBorrowCounts[i],
                HasAccount = hasAccountList[i]
            };
            return dto;
        }).ToList();

        _logger.LogInformation("Successfully found {Count} members out of {Total} total.", items.Count, total);

        return PagedResult<MemberDto>.Create(dtos, total, request.Page, request.PageSize);
    }
}
