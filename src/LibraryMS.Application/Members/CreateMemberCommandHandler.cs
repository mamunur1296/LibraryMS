using LibraryMS.Application.Contracts.DTOs.Member;
using LibraryMS.Application.Contracts.Members;
using LibraryMS.Application.Mapping;
using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.Shared;
using LibraryMS.Domain.Shared.Guards;
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
        MemberManager manager,
        IMemberRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<CreateMemberCommandHandler> logger)
    {
        _manager = manager;
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<MemberDto> Handle(CreateMemberCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating member: {FirstName} {LastName}, Email: {Email}", request.FirstName, request.LastName, request.Email);

        var member = await _manager.CreateAsync(
            request.FirstName, request.LastName, request.Email,
            request.Phone, request.Address, cancellationToken);

        var dbFailed = false;
        try
        {
            await _repository.AddAsync(member, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save member {Name} to database.", member.FullName);
            dbFailed = true;
        }

        Ensure.Against(dbFailed, "An error occurred while saving the member to the database.", "DB_UPDATE_ERROR");

        _logger.LogInformation("Member '{Name}' created successfully with membership number {Number}",
            member.FullName, member.MembershipNumber);

        return member.ToDto();
    }
}
