using LibraryMS.Application.Mapping;
using LibraryMS.Application.Contracts.Borrows;
using LibraryMS.Application.Contracts.DTOs.Borrow;
using LibraryMS.Domain.BorrowManagement;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryMS.Application.Borrows;

public sealed class GetBorrowByIdQueryHandler : IRequestHandler<GetBorrowByIdQuery, BorrowDto?>
{
    private readonly IBorrowRepository _repository;

    public GetBorrowByIdQueryHandler(IBorrowRepository repository)
    {
        _repository = repository; 
    }

    public async Task<BorrowDto?> Handle(GetBorrowByIdQuery request, CancellationToken cancellationToken)
    {
        var record = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return record?.ToDto();
    }
}
