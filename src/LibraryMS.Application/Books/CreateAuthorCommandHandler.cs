using LibraryMS.Application.Contracts.Books;
using LibraryMS.Application.Contracts.DTOs.Book;
using LibraryMS.Application.Mapping;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.Shared.Guards;
using MediatR;

namespace LibraryMS.Application.Books;

public sealed class CreateAuthorCommandHandler : IRequestHandler<CreateAuthorCommand, AuthorDto>
{
    private readonly IBookRepository _repository;
    private readonly AuthorManager _authorManager;

    public CreateAuthorCommandHandler(IBookRepository repository, AuthorManager authorManager)
    {
        _repository = repository;
        _authorManager = authorManager;
    }

    public async Task<AuthorDto> Handle(CreateAuthorCommand request, CancellationToken cancellationToken)
    {
        var author = _authorManager.Create(request.Name, request.Biography);
        var dbFailed = false;
        var innerMsg = string.Empty;
        try
        {
            await _repository.AddAuthorAsync(author, cancellationToken);
        }
        catch (Exception ex)
        {
            dbFailed = true;
            innerMsg = ex.InnerException?.Message ?? ex.Message;
        }

        Ensure.Against(dbFailed, $"Failed to save author. Error: {innerMsg}", "DB_UPDATE_ERROR");
        return author.ToDto();
    }
}
