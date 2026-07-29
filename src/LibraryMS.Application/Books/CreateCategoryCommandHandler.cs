using LibraryMS.Application.Mapping;
using LibraryMS.Application.Contracts.Books;
using LibraryMS.Application.Contracts.DTOs.Book;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.Shared.Guards;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryMS.Application.Books;

public sealed class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    private readonly IBookRepository _repository;
    private readonly ILogger<CreateCategoryCommandHandler> _logger;

    public CreateCategoryCommandHandler(
        IBookRepository repository,
        ILogger<CreateCategoryCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating category with Name: {Name}", request.Name);

        var category = new Category(Guid.NewGuid(), request.Name, request.Description);
        var dbFailed = false;
        var innerMsg = string.Empty;

        try
        {
            await _repository.AddCategoryAsync(category, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save category {Name} to database.", request.Name);
            dbFailed = true;
            innerMsg = ex.InnerException?.Message ?? ex.Message;
        }

        Ensure.Against(dbFailed, $"Failed to save category. Error: {innerMsg}", "DB_UPDATE_ERROR");

        _logger.LogInformation("Category '{Name}' created successfully with ID {CategoryId}", category.Name, category.Id);

        return category.ToDto();
    }
}
