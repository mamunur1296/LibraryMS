using LibraryMS.Application.Contracts.Books;
using LibraryMS.Application.Contracts.DTOs.Book;
using LibraryMS.Application.Mapping;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.BookManagement.AggregateRoots;
using LibraryMS.Domain.BookManagement.Entities;
using LibraryMS.Domain.BookManagement.Services;
using LibraryMS.Domain.Shared.Guards;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Books;

public sealed class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    private readonly IBookRepository _repository;
    private readonly CategoryManager _categoryManager;
    private readonly ILogger<CreateCategoryCommandHandler> _logger;

    public CreateCategoryCommandHandler(
        IBookRepository repository,
        CategoryManager categoryManager,
        ILogger<CreateCategoryCommandHandler> logger)
    {
        _repository = repository;
        _categoryManager = categoryManager;
        _logger = logger;
    }

    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating category with Name: {Name}", request.Name);

        var category = _categoryManager.Create(request.Name, request.Description);
        var dbFailed = false;
        try
        {
            await _repository.AddCategoryAsync(category, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save category {Name} to database.", request.Name);
            dbFailed = true;
        }

        Ensure.Against(dbFailed, "An error occurred while saving the category to the database.", "DB_UPDATE_ERROR");

        _logger.LogInformation("Category '{Name}' created successfully with ID {CategoryId}", category.Name, category.Id);

        return category.ToDto();
    }
}

