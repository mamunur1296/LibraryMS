using LibraryMS.Application.Contracts.Books;
using LibraryMS.Application.Contracts.DTOs.Book;
using LibraryMS.Application.Mapping;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.BookManagement.AggregateRoots;
using LibraryMS.Domain.BookManagement.Entities;
using LibraryMS.Domain.BookManagement.Services;
using LibraryMS.Domain.Shared;
using LibraryMS.Domain.Shared.Guards;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Books;

public sealed class AddBookCopiesCommandHandler : IRequestHandler<AddBookCopiesCommand, List<BookCopyDto>>
{
    private readonly BookCopyManager _copyManager;
    private readonly IBookRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddBookCopiesCommandHandler> _logger;

    public AddBookCopiesCommandHandler(
        BookCopyManager copyManager,
        IBookRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<AddBookCopiesCommandHandler> logger)
    {
        _copyManager = copyManager;
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<List<BookCopyDto>> Handle(AddBookCopiesCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Adding {Quantity} copies to Book {BookId} at Branch {BranchId}", request.Quantity, request.BookId, request.BranchId);

        var book = await _repository.GetByIdWithCopiesAsync(request.BookId, cancellationToken);
        Ensure.Found(book, $"Book with ID '{request.BookId}' was not found.");

        var addedCopies = new List<BookCopy>();
        for (int i = 0; i < request.Quantity; i++)
        {
            var copy = _copyManager.AddCopyToBranch(book!, request.BranchId);
            addedCopies.Add(copy);
        }

        await _repository.UpdateAsync(book!, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully added {Quantity} copies to Book {BookId}", request.Quantity, request.BookId);

        return addedCopies.Select(c => c.ToDto()).ToList();
    }
}

