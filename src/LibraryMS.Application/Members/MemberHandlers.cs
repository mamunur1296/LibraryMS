using LibraryMS.Application.Mapping;
using LibraryMS.Application.Contracts.Members;
using LibraryMS.Application.Contracts.Common;
using LibraryMS.Application.Contracts.DTOs.Member;
using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.Shared.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Members;

public sealed class CreateMemberCommandHandler : IRequestHandler<CreateMemberCommand, MemberDto>
{
    private readonly MemberManager _manager;
    private readonly IMemberRepository _repository;
    
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateMemberCommandHandler> _logger;

    public CreateMemberCommandHandler(
        MemberManager manager, IMemberRepository repository,
        IUnitOfWork unitOfWork, ILogger<CreateMemberCommandHandler> logger)
    {
        _manager = manager; _repository = repository;
        _unitOfWork = unitOfWork; _logger = logger;
    }

    public async Task<MemberDto> Handle(CreateMemberCommand request, CancellationToken cancellationToken)
    {
        var member = await _manager.CreateAsync(
            request.FirstName, request.LastName, request.Email,
            request.Phone, request.Address, cancellationToken);

        await _repository.AddAsync(member, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Member '{Name}' created with membership number {Number}",
            member.FullName, member.MembershipNumber);

        return member.ToDto();
    }
}

public sealed class UpdateMemberCommandHandler : IRequestHandler<UpdateMemberCommand, MemberDto>
{
    private readonly MemberManager _manager;
    private readonly IMemberRepository _repository;
    
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMemberCommandHandler(
        MemberManager manager, IMemberRepository repository,
        IUnitOfWork unitOfWork)
    {
        _manager = manager; _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<MemberDto> Handle(UpdateMemberCommand request, CancellationToken cancellationToken)
    {
        var member = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Member), request.Id);

        await _manager.UpdateAsync(member, request.FirstName, request.LastName,
            request.Phone, request.Address, cancellationToken);

        await _repository.UpdateAsync(member, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return member.ToDto();
    }
}

public sealed class SuspendMemberCommandHandler : IRequestHandler<SuspendMemberCommand, MemberDto>
{
    private readonly MemberManager _manager;
    private readonly IMemberRepository _repository;
    
    private readonly IUnitOfWork _unitOfWork;

    public SuspendMemberCommandHandler(
        MemberManager manager, IMemberRepository repository,
        IUnitOfWork unitOfWork)
    {
        _manager = manager; _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<MemberDto> Handle(SuspendMemberCommand request, CancellationToken cancellationToken)
    {
        var member = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Member), request.Id);

        _manager.SuspendMember(member, request.SuspendedUntil, request.Reason);
        await _repository.UpdateAsync(member, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return member.ToDto();
    }
}

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

public sealed class GetMemberByIdQueryHandler : IRequestHandler<GetMemberByIdQuery, MemberDto?>
{
    private readonly IMemberRepository _repository;
    

    public GetMemberByIdQueryHandler(IMemberRepository repository)
    {
        _repository = repository; 
    }

    public async Task<MemberDto?> Handle(GetMemberByIdQuery request, CancellationToken cancellationToken)
    {
        var member = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return member?.ToDto();
    }
}
