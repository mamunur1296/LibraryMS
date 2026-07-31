using LibraryMS.Application.Contracts.Books;
using LibraryMS.Application.Contracts.DTOs.Book;
using LibraryMS.Application.Mapping;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.BookManagement.Services;
using LibraryMS.Domain.Shared.Guards;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Books;

public sealed class CreateAuthorCommandHandler : IRequestHandler<CreateAuthorCommand, AuthorDto>
{
    private readonly IBookRepository _repository;
    private readonly AuthorManager _authorManager;
    private readonly ILogger<CreateAuthorCommandHandler> _logger;

    public CreateAuthorCommandHandler(
        IBookRepository repository,
        AuthorManager authorManager,
        ILogger<CreateAuthorCommandHandler> logger)
    {
        _repository = repository;
        _authorManager = authorManager;
        _logger = logger;
    }

    public async Task<AuthorDto> Handle(CreateAuthorCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating author with Name: {Name}", request.Name);

        var author = _authorManager.Create(request.Name, request.Biography);
        var dbFailed = false;
        try
        {
            await _repository.AddAuthorAsync(author, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to persist author {Name} to database.", request.Name);
            dbFailed = true;
        }

        Ensure.Against(dbFailed, "An error occurred while saving the author to the database.", "DB_UPDATE_ERROR");

        _logger.LogInformation("Author '{Name}' created successfully with ID {AuthorId}", author.Name, author.Id);

        return author.ToDto();
    }
}

