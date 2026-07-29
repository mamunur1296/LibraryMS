using LibraryMS.Domain.Common;
using LibraryMS.Domain.Shared.Exceptions;
using System.Threading;
using System.Threading.Tasks;
using System;

using LibraryMS.Domain.BookManagement.AggregateRoots;
using LibraryMS.Domain.BookManagement.Entities;

namespace LibraryMS.Domain.BookManagement.Services;

/// <summary>Domain service for creating and managing Book aggregates.</summary>
public sealed class BookManager
{
    private readonly IBookRepository _repository;
    private readonly IGuidGenerator _guidGenerator;

    public BookManager(IBookRepository repository, IGuidGenerator guidGenerator)
    {
        _repository = repository;
        _guidGenerator = guidGenerator;
    }

    public async Task<Book> CreateAsync(
        string title, string isbn, string? description,
        int publicationYear, Guid categoryId, Guid authorId, string language,
        CancellationToken ct = default)
    {
        await EnsureIsbnUniqueAsync(isbn, excludeId: null, ct);

        return new Book(_guidGenerator.Create(), title, isbn, description, publicationYear, categoryId, authorId, language);
    }

    public async Task<Book> UpdateAsync(
        Book book, string title, string? description,
        int publicationYear, Guid categoryId, Guid authorId, string language,
        CancellationToken ct = default)
    {
        book.Update(title, description, publicationYear, categoryId, authorId, language);
        return book;
    }

    private async Task EnsureIsbnUniqueAsync(string isbn, Guid? excludeId, CancellationToken ct)
    {
        var exists = await _repository.IsbnExistsAsync(isbn, excludeId, ct);
        if (exists)
            throw new DomainException($"A book with ISBN '{isbn}' already exists.", "BOOK_DUPLICATE_ISBN");
    }
}
