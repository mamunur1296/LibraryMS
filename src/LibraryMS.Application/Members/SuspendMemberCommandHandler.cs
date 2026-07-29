using LibraryMS.Application.Contracts.DTOs.Member;
using LibraryMS.Application.Contracts.Members;
using LibraryMS.Application.Mapping;
using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.Shared;
using LibraryMS.Domain.Shared.Guards;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Members;

public sealed class SuspendMemberCommandHandler : IRequestHandler<SuspendMemberCommand, MemberDto>
{
    private readonly MemberManager _manager;
    private readonly IMemberRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SuspendMemberCommandHandler> _logger;

    public SuspendMemberCommandHandler(
        MemberManager manager,
        IMemberRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<SuspendMemberCommandHandler> logger)
    {
        _manager = manager;
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<MemberDto> Handle(SuspendMemberCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Attempting to suspend member with ID {Id} until {Date} for reason: {Reason}",
            request.Id, request.SuspendedUntil, request.Reason);

        var member = await _repository.GetByIdAsync(request.Id, cancellationToken);
        Ensure.Found(member, $"Member with ID '{request.Id}' was not found.");

        _manager.SuspendMember(member!, request.SuspendedUntil, request.Reason);
        await _repository.UpdateAsync(member!, cancellationToken);

        var dbFailed = false;
        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to suspend member '{Name}' with ID {Id} in database.", member!.FullName, request.Id);
            dbFailed = true;
        }

        Ensure.Against(dbFailed, "An error occurred while suspending the member in the database.", "DB_UPDATE_ERROR");

        _logger.LogInformation("Member '{Name}' suspended until {Date} for reason: {Reason}",
            member!.FullName, request.SuspendedUntil, request.Reason);

        return member.ToDto();
    }
}
