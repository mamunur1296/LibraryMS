using LibraryMS.Application.Contracts.Books;
using LibraryMS.Application.Contracts.DTOs.Book;
using LibraryMS.Application.Mapping;
using LibraryMS.Domain.BookManagement;
using MediatR;

namespace LibraryMS.Application.Books;

public sealed class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, List<CategoryDto>>
{
    private readonly IBookRepository _repository;

    public GetAllCategoriesQueryHandler(IBookRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _repository.GetAllCategoriesAsync(cancellationToken);
        return categories.Select(c => c.ToDto()).ToList();
    }
}

