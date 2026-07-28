using LibraryMS.Application.Mapping;
using LibraryMS.Application.Contracts.Books;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.Shared.Exceptions;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryMS.Application.Books;

public sealed class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    private readonly IBookRepository _repository;

    public CreateCategoryCommandHandler(IBookRepository repository)
    {
        _repository = repository;
    }

    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new Category(Guid.NewGuid(), request.Name, request.Description);
        try
        {
            await _repository.AddCategoryAsync(category, cancellationToken);
        }
        catch (Exception ex)
        {
            var innerMsg = ex.InnerException?.Message ?? ex.Message;
            throw new DomainException($"Failed to save category. Error: {innerMsg}", "DB_UPDATE_ERROR");
        }
        return category.ToDto();
    }
}
