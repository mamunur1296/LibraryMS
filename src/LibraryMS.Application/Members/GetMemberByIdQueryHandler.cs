using LibraryMS.Application.Mapping;
using LibraryMS.Application.Contracts.Members;
using LibraryMS.Application.Contracts.DTOs.Member;
using LibraryMS.Domain.MemberManagement;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryMS.Application.Members;

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
