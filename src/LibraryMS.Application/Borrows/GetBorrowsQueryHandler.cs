using LibraryMS.Application.Mapping;
using LibraryMS.Application.Contracts.Borrows;
using LibraryMS.Application.Contracts.DTOs.Borrow;
using LibraryMS.Application.Contracts.Common;
using LibraryMS.Domain.BorrowManagement;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryMS.Application.Borrows;

public sealed class GetBorrowsQueryHandler : IRequestHandler<GetBorrowsQuery, PagedResult<BorrowDto>>
{
    private readonly IBorrowRepository _repository;

    public GetBorrowsQueryHandler(IBorrowRepository repository)
    {
        _repository = repository; 
    }

    public async Task<PagedResult<BorrowDto>> Handle(GetBorrowsQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await _repository.GetPagedAsync(
            request.MemberId, request.BookId, request.Status,
            request.Page, request.PageSize, cancellationToken);

        return PagedResult<BorrowDto>.Create(
            items.Select(i => i.ToDto()).ToList(),
            total, request.Page, request.PageSize);
    }
}
