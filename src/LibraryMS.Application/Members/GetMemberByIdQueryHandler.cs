using LibraryMS.Application.Contracts.DTOs.Member;
using LibraryMS.Application.Contracts.Members;
using LibraryMS.Application.Mapping;
using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.MemberManagement.AggregateRoots;
using LibraryMS.Domain.MemberManagement.Services;
using LibraryMS.Domain.Shared.Guards;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Members;

public sealed class GetMemberByIdQueryHandler : IRequestHandler<GetMemberByIdQuery, MemberDto?>
{
    private readonly IMemberRepository _repository;
    private readonly ILogger<GetMemberByIdQueryHandler> _logger;

    public GetMemberByIdQueryHandler(IMemberRepository repository, ILogger<GetMemberByIdQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<MemberDto?> Handle(GetMemberByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving member with ID: {Id}", request.Id);

        var member = await _repository.GetByIdAsync(request.Id, cancellationToken);
        Ensure.Found(member, $"Member with ID '{request.Id}' was not found.");

        _logger.LogInformation("Successfully retrieved member with ID: {Id}", request.Id);

        return member?.ToDto();
    }
}

