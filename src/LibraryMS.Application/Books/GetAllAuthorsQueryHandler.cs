using LibraryMS.Application.Mapping;
using LibraryMS.Application.Contracts.Books;
using LibraryMS.Application.Contracts.DTOs.Book;
using LibraryMS.Domain.BookManagement;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
