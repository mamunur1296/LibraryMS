using LibraryMS.Domain.Shared;
using LibraryMS.Application.Mapping;
using LibraryMS.Application.Contracts.Books;
using LibraryMS.Application.Contracts.Common;
using LibraryMS.Application.Contracts.DTOs.Book;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.Shared.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Books;

public sealed class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, BookDto>
{
    private readonly BookManager _manager;
    private readonly IBookRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateBookCommandHandler> _logger;

    public CreateBookCommandHandler(
        BookManager manager, IBookRepository repository,
        IUnitOfWork unitOfWork, ILogger<CreateBookCommandHandler> logger)
    {
        _manager = manager; _repository = repository;
        _unitOfWork = unitOfWork; _logger = logger;
    }

    public async Task<BookDto> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        var book = await _manager.CreateAsync(
            request.Title, request.ISBN, request.Description,
            request.PublicationYear, request.CategoryId, request.AuthorId,
            request.Language, cancellationToken);

        // Add initial copies to the specified branch
        for (int i = 0; i < request.InitialCopies; i++)
            _manager.AddCopyToBranch(book, request.BranchId);

        await _repository.AddAsync(book, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Book '{Title}' (ISBN: {ISBN}) created with {Copies} copies",
            book.Title, book.ISBN.Value, request.InitialCopies);

        return book.ToDto();
    }
}

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

public sealed class DeleteBookCommandHandler : IRequestHandler<DeleteBookCommand>
{
    private readonly IBookRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBookCommandHandler(IBookRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository; _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteBookCommand request, CancellationToken cancellationToken)
    {
        var book = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Book), request.Id);

        await _repository.DeleteAsync(book, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

public sealed class AddBookCopiesCommandHandler : IRequestHandler<AddBookCopiesCommand, List<BookCopyDto>>
{
    private readonly BookManager _manager;
    private readonly IBookRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public AddBookCopiesCommandHandler(
        BookManager manager, IBookRepository repository,
        IUnitOfWork unitOfWork)
    {
        _manager = manager; _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<BookCopyDto>> Handle(AddBookCopiesCommand request, CancellationToken cancellationToken)
    {
        var book = await _repository.GetByIdWithCopiesAsync(request.BookId, cancellationToken)
            ?? throw new NotFoundException(nameof(Book), request.BookId);

        var addedCopies = new List<BookCopy>();
        for (int i = 0; i < request.Quantity; i++)
        {
            var copy = _manager.AddCopyToBranch(book, request.BranchId);
            addedCopies.Add(copy);
        }

        await _repository.UpdateAsync(book, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return addedCopies.Select(c => c.ToDto()).ToList();
    }
}

public sealed class SearchBooksQueryHandler : IRequestHandler<SearchBooksQuery, PagedResult<BookDto>>
{
    private readonly IBookRepository _repository;

    public SearchBooksQueryHandler(IBookRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<BookDto>> Handle(SearchBooksQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await _repository.SearchAsync(
            request.SearchTerm, request.CategoryId, request.AuthorId,
            request.BranchId, request.Page, request.PageSize, cancellationToken);

        return PagedResult<BookDto>.Create(
            items.Select(i => i.ToDto()).ToList(),
            total, request.Page, request.PageSize);
    }
}

public sealed class GetAvailableCopiesQueryHandler : IRequestHandler<GetAvailableCopiesQuery, List<BookCopyDto>>
{
    private readonly IBookRepository _repository;

    public GetAvailableCopiesQueryHandler(IBookRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<BookCopyDto>> Handle(GetAvailableCopiesQuery request, CancellationToken cancellationToken)
    {
        var book = await _repository.GetByIdWithCopiesAsync(request.BookId, cancellationToken);
        if (book == null) return new List<BookCopyDto>();
        return book.Copies
            .Where(c => c.Status == LibraryMS.Domain.Shared.Enums.CopyStatus.Available)
            .Select(c => c.ToDto())
            .ToList();
    }
}

public sealed class GetBookByIdQueryHandler : IRequestHandler<GetBookByIdQuery, BookDto?>
{
    private readonly IBookRepository _repository;

    public GetBookByIdQueryHandler(IBookRepository repository)
    {
        _repository = repository;
    }

    public async Task<BookDto?> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
    {
        var book = await _repository.GetByIdWithCopiesAsync(request.Id, cancellationToken);
        return book?.ToDto();
    }
}
