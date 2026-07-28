using LibraryMS.Domain.Shared;
using LibraryMS.Application.Mapping;
using LibraryMS.Application.Contracts.Members;
using LibraryMS.Application.Contracts.DTOs.Member;
using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.Shared.Guards;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryMS.Application.Members;

public sealed class SuspendMemberCommandHandler : IRequestHandler<SuspendMemberCommand, MemberDto>
{
    private readonly MemberManager _manager;
    private readonly IMemberRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SuspendMemberCommandHandler> _logger;

    public SuspendMemberCommandHandler(
        MemberManager manager, IMemberRepository repository,
        IUnitOfWork unitOfWork, ILogger<SuspendMemberCommandHandler> logger)
    {
        _manager = manager; _repository = repository;
        _unitOfWork = unitOfWork; _logger = logger;
    }

    public async Task<MemberDto> Handle(SuspendMemberCommand request, CancellationToken cancellationToken)
    {
        var member = await _repository.GetByIdAsync(request.Id, cancellationToken);
        Ensure.Found(member, $"Member with ID '{request.Id}' was not found.");

        _manager.SuspendMember(member!, request.SuspendedUntil, request.Reason);
        await _repository.UpdateAsync(member!, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Member '{Name}' suspended until {Date} for reason: {Reason}",
            member!.FullName, request.SuspendedUntil, request.Reason);

        return member.ToDto();
    }
}
