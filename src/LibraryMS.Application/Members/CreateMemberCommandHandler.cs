using LibraryMS.Domain.Shared;
using LibraryMS.Application.Mapping;
using LibraryMS.Application.Contracts.Members;
using LibraryMS.Application.Contracts.DTOs.Member;
using LibraryMS.Domain.MemberManagement;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

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
