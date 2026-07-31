using LibraryMS.Application.Contracts.Books;
using LibraryMS.Application.Contracts.DTOs.Book;
using LibraryMS.Application.Mapping;
using LibraryMS.Domain.BookManagement;
using MediatR;

namespace LibraryMS.Application.Books;

public sealed class GetAvailableCopiesQueryHandler : IRequestHandler<GetAvailableCopiesQuery, List<BookCopyDto>>
{
    private readonly IBookRepository _repository;

    public GetAvailableCopiesQueryHandler(IBookRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<BookCopyDto>> Handle(GetAvailableCopiesQuery request, CancellationToken cancellationToken)
    {
        var book = await _repository.GetByIdWithCopiesAsync(request.BookId, cancellationToken);
        if (book == null) return new List<BookCopyDto>();
        return book.Copies
            .Where(c => c.Status == LibraryMS.Domain.Shared.Enums.CopyStatus.Available)
            .Where(c => request.BranchId == null || c.BranchId == request.BranchId.Value)
            .Select(c => c.ToDto())
            .ToList();
    }
}

