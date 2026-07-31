using LibraryMS.Application.Contracts.DTOs.Member;
using LibraryMS.Application.Contracts.Members;
using LibraryMS.Application.Mapping;
using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.MemberManagement.Services;
using LibraryMS.Domain.Shared;
using LibraryMS.Domain.Shared.Guards;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Members;

public sealed class ActivateMemberCommandHandler : IRequestHandler<ActivateMemberCommand, MemberDto>
{
    private readonly MemberManager _manager;
    private readonly IMemberRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ActivateMemberCommandHandler> _logger;

    public ActivateMemberCommandHandler(
        MemberManager manager,
        IMemberRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<ActivateMemberCommandHandler> logger)
    {
        _manager = manager;
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<MemberDto> Handle(ActivateMemberCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Activating member with ID: {Id}", request.Id);

        var member = await _repository.GetByIdAsync(request.Id, cancellationToken);
        Ensure.Found(member, $"Member with ID '{request.Id}' was not found.");

        _manager.ActivateMember(member!);
        await _repository.UpdateAsync(member!, cancellationToken);

        var dbFailed = false;
        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to activate member with ID {Id} in database.", request.Id);
            dbFailed = true;
        }

        Ensure.Against(dbFailed, "An error occurred while activating the member in the database.", "DB_UPDATE_ERROR");

        _logger.LogInformation("Member '{Name}' activated successfully.", member!.FullName);

        return member.ToDto();
    }
}
