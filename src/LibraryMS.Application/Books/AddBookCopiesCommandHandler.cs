using LibraryMS.Application.Contracts.Books;
using LibraryMS.Application.Contracts.DTOs.Book;
using LibraryMS.Application.Mapping;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.Shared;
using LibraryMS.Domain.Shared.Guards;
using MediatR;

namespace LibraryMS.Application.Books;

public sealed class AddBookCopiesCommandHandler : IRequestHandler<AddBookCopiesCommand, List<BookCopyDto>>
{
    private readonly BookManager _manager;
    private readonly IBookRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public AddBookCopiesCommandHandler(
        BookManager manager,
        IBookRepository repository,
        IUnitOfWork unitOfWork)
    {
        _manager = manager;
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<BookCopyDto>> Handle(AddBookCopiesCommand request, CancellationToken cancellationToken)
    {
        var book = await _repository.GetByIdWithCopiesAsync(request.BookId, cancellationToken);
        Ensure.Found(book, $"Book with ID '{request.BookId}' was not found.");

        var addedCopies = new List<BookCopy>();
        for (int i = 0; i < request.Quantity; i++)
        {
            var copy = _manager.AddCopyToBranch(book!, request.BranchId);
            addedCopies.Add(copy);
        }

        await _repository.UpdateAsync(book!, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return addedCopies.Select(c => c.ToDto()).ToList();
    }
}
