using LibraryMS.Domain.Shared;
using LibraryMS.Application.Contracts.Books;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.BookManagement.AggregateRoots;
using LibraryMS.Domain.BookManagement.Entities;
using LibraryMS.Domain.BookManagement.Services;
using LibraryMS.Domain.Shared.Guards;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryMS.Application.Books;

public sealed class DeleteBookCommandHandler : IRequestHandler<DeleteBookCommand>
{
    private readonly IBookRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteBookCommandHandler> _logger;

    public DeleteBookCommandHandler(
        IBookRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteBookCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(DeleteBookCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting book with ID: {BookId}", request.Id);

        var book = await _repository.GetByIdAsync(request.Id, cancellationToken);
        Ensure.Found(book, $"Book with ID '{request.Id}' was not found.");

        await _repository.DeleteAsync(book!, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Book with ID {BookId} ('{Title}') deleted successfully.", request.Id, book!.Title);
    }
}

