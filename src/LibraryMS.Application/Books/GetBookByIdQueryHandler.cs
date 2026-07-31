using LibraryMS.Application.Contracts.Books;
using LibraryMS.Application.Contracts.DTOs.Book;
using LibraryMS.Application.Mapping;
using LibraryMS.Domain.BookManagement;
using MediatR;

namespace LibraryMS.Application.Books;

public sealed class GetBookByIdQueryHandler : IRequestHandler<GetBookByIdQuery, BookDto?>
{
    private readonly IBookRepository _repository;

    public GetBookByIdQueryHandler(IBookRepository repository)
    {
        _repository = repository;
    }

    public async Task<BookDto?> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
    {
        var book = await _repository.GetByIdWithCopiesAsync(request.Id, cancellationToken);
        return book?.ToDto();
    }
}

