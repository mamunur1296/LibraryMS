using LibraryMS.Application.Contracts.Books;
using LibraryMS.Application.Contracts.DTOs.Book;
using LibraryMS.Application.Mapping;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.BranchManagement;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryMS.Application.Books;

public sealed class GetBookCopiesQueryHandler : IRequestHandler<GetBookCopiesQuery, List<BookCopyDto>>
{
    private readonly IBookRepository _repository;
    private readonly IBranchRepository _branchRepository;

    public GetBookCopiesQueryHandler(IBookRepository repository, IBranchRepository branchRepository)
    {
        _repository = repository;
        _branchRepository = branchRepository;
    }

    public async Task<List<BookCopyDto>> Handle(GetBookCopiesQuery request, CancellationToken cancellationToken)
    {
        var book = await _repository.GetByIdWithCopiesAsync(request.BookId, cancellationToken);
        if (book == null) return new List<BookCopyDto>();

        var branchIds = book.Copies.Select(c => c.BranchId).Distinct().ToList();
        var branches = await _branchRepository.GetByIdsAsync(branchIds, cancellationToken);
        var branchDict = branches.ToDictionary(b => b.Id);

        return book.Copies
            .Select(c => c.ToDto(branchDict.TryGetValue(c.BranchId, out var branch) ? branch : null))
            .ToList();
    }
}
