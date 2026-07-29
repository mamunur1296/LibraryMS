using LibraryMS.Application.Contracts.Books;
using LibraryMS.Application.Contracts.DTOs.Book;
using LibraryMS.Application.Mapping;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.Shared;
using LibraryMS.Domain.Shared.Guards;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Books;

public sealed class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand, BookDto>
{
    private readonly BookManager _manager;
    private readonly IBookRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateBookCommandHandler> _logger;

    public UpdateBookCommandHandler(
        BookManager manager,
        IBookRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateBookCommandHandler> logger)
    {
        _manager = manager;
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<BookDto> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating book with ID: {BookId}", request.Id);

        var book = await _repository.GetByIdWithCopiesAsync(request.Id, cancellationToken);
        Ensure.Found(book, $"Book with ID '{request.Id}' was not found.");

        await _manager.UpdateAsync(book!, request.Title, request.Description,
            request.PublicationYear, request.CategoryId, request.AuthorId,
            request.Language, cancellationToken);

        await _repository.UpdateAsync(book!, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Book with ID {BookId} updated successfully.", request.Id);

        return book!.ToDto();
    }
}
