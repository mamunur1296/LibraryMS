using LibraryMS.Application.Contracts.Books;
using LibraryMS.Application.Contracts.DTOs.Book;
using LibraryMS.Application.Mapping;
using LibraryMS.Domain.BookManagement;
using MediatR;

namespace LibraryMS.Application.Books;

public sealed class GetAllAuthorsQueryHandler : IRequestHandler<GetAllAuthorsQuery, List<AuthorDto>>
{
    private readonly IBookRepository _repository;

    public GetAllAuthorsQueryHandler(IBookRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<AuthorDto>> Handle(GetAllAuthorsQuery request, CancellationToken cancellationToken)
    {
        var authors = await _repository.GetAllAuthorsAsync(cancellationToken);
        return authors.Select(a => a.ToDto()).ToList();
    }
}

