using LibraryMS.Application.Mapping;
using LibraryMS.Application.Contracts.Books;
using LibraryMS.Application.Contracts.DTOs.Book;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.Shared;
using LibraryMS.Domain.Shared.Exceptions;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryMS.Application.Books;

public sealed class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand, BookDto>
{
    private readonly BookManager _manager;
    private readonly IBookRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBookCommandHandler(
        BookManager manager, IBookRepository repository,
        IUnitOfWork unitOfWork)
    {
        _manager = manager; _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<BookDto> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
    {
        var book = await _repository.GetByIdWithCopiesAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Book), request.Id);

        await _manager.UpdateAsync(book, request.Title, request.Description,
            request.PublicationYear, request.CategoryId, request.AuthorId,
            request.Language, cancellationToken);

        await _repository.UpdateAsync(book, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return book.ToDto();
    }
}
