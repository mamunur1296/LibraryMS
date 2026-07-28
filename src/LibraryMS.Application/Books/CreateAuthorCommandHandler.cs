using LibraryMS.Application.Mapping;
using LibraryMS.Application.Contracts.Books;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.Shared.Exceptions;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryMS.Application.Books;

public sealed class CreateAuthorCommandHandler : IRequestHandler<CreateAuthorCommand, AuthorDto>
{
    private readonly IBookRepository _repository;

    public CreateAuthorCommandHandler(IBookRepository repository)
    {
        _repository = repository;
    }

    public async Task<AuthorDto> Handle(CreateAuthorCommand request, CancellationToken cancellationToken)
    {
        var author = new Author(Guid.NewGuid(), request.Name, request.Biography);
        try
        {
            await _repository.AddAuthorAsync(author, cancellationToken);
        }
        catch (Exception ex)
        {
            var innerMsg = ex.InnerException?.Message ?? ex.Message;
            throw new DomainException($"Failed to save author. Error: {innerMsg}", "DB_UPDATE_ERROR");
        }
        return author.ToDto();
    }
}
