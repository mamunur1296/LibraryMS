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

public sealed class UpdateMemberCommandHandler : IRequestHandler<UpdateMemberCommand, MemberDto>
{
    private readonly MemberManager _manager;
    private readonly IMemberRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateMemberCommandHandler> _logger;

    public UpdateMemberCommandHandler(
        MemberManager manager, IMemberRepository repository,
        IUnitOfWork unitOfWork, ILogger<UpdateMemberCommandHandler> logger)
    {
        _manager = manager; _repository = repository;
        _unitOfWork = unitOfWork; _logger = logger;
    }

    public async Task<MemberDto> Handle(UpdateMemberCommand request, CancellationToken cancellationToken)
    {
        var member = await _repository.GetByIdAsync(request.Id, cancellationToken);
        Ensure.Found(member, $"Member with ID '{request.Id}' was not found.");

        await _manager.UpdateAsync(member!, request.FirstName, request.LastName,
            request.Phone, request.Address, cancellationToken);

        await _repository.UpdateAsync(member!, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Member '{Name}' updated successfully", member!.FullName);

        return member.ToDto();
    }
}
