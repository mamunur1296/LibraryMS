using LibraryMS.Application.Contracts.DTOs.Member;
using LibraryMS.Application.Contracts.Members;
using LibraryMS.Application.Mapping;
using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.MemberManagement.AggregateRoots;
using LibraryMS.Domain.MemberManagement.Services;
using LibraryMS.Domain.Shared;
using LibraryMS.Domain.Shared.Guards;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Members;

public sealed class UpdateMemberCommandHandler : IRequestHandler<UpdateMemberCommand, MemberDto>
{
    private readonly MemberManager _manager;
    private readonly IMemberRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateMemberCommandHandler> _logger;

    public UpdateMemberCommandHandler(
        MemberManager manager,
        IMemberRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateMemberCommandHandler> logger)
    {
        _manager = manager;
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<MemberDto> Handle(UpdateMemberCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating member with ID: {Id}, Name: {FirstName} {LastName}", request.Id, request.FirstName, request.LastName);

        var member = await _repository.GetByIdAsync(request.Id, cancellationToken);
        Ensure.Found(member, $"Member with ID '{request.Id}' was not found.");

        await _manager.UpdateAsync(member!, request.FirstName, request.LastName,
            request.Phone, request.Address, cancellationToken);

        await _repository.UpdateAsync(member!, cancellationToken);

        var dbFailed = false;
        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update member '{Name}' with ID {Id} in database.", member!.FullName, request.Id);
            dbFailed = true;
        }

        Ensure.Against(dbFailed, "An error occurred while updating the member in the database.", "DB_UPDATE_ERROR");

        _logger.LogInformation("Member '{Name}' updated successfully", member!.FullName);

        return member.ToDto();
    }
}

