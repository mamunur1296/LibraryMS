using LibraryMS.Domain.Shared;
using LibraryMS.Application.Mapping;
using LibraryMS.Application.Contracts.Books;
using LibraryMS.Application.Contracts.DTOs.Book;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.Shared.Exceptions;
using LibraryMS.Domain.Shared.Guards;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryMS.Application.Books;

public sealed class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, BookDto>
{
    private readonly BookManager _manager;
    private readonly IBookRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateBookCommandHandler> _logger;

    public CreateBookCommandHandler(
        BookManager manager,
        IBookRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<CreateBookCommandHandler> logger)
    {
        _manager = manager;
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<BookDto> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating book with Title: {Title}, ISBN: {ISBN}, InitialCopies: {InitialCopies}",
            request.Title, request.ISBN, request.InitialCopies);

        var book = await _manager.CreateAsync(
            request.Title, request.ISBN, request.Description,
            request.PublicationYear, request.CategoryId, request.AuthorId,
            request.Language, cancellationToken);

        // Add initial copies to the specified branch
        for (int i = 0; i < request.InitialCopies; i++)
            _manager.AddCopyToBranch(book, request.BranchId);

        var dbFailed = false;
        var innerMsg = string.Empty;
        try
        {
            await _repository.AddAsync(book, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database constraint violation while creating book '{Title}'.", request.Title);
            dbFailed = true;
            innerMsg = ex.InnerException?.Message ?? ex.Message;
        }

        Ensure.Against(dbFailed, $"Failed to save book to database. Error: {innerMsg}", "DB_UPDATE_ERROR");

        _logger.LogInformation("Book '{Title}' (ISBN: {ISBN}) created successfully with {Copies} copies",
            book.Title, book.ISBN.Value, request.InitialCopies);

        return book.ToDto();
    }
}
