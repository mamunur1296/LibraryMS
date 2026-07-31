using LibraryMS.Domain.BookManagement.AggregateRoots;
using LibraryMS.Domain.BookManagement.Entities;

namespace LibraryMS.Domain.BookManagement;

// Repository contract for Book aggregate.
public interface IBookRepository
{
    Task<Book?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<Book>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default);
    Task<Book?> GetByIdWithCopiesAsync(Guid id, CancellationToken ct = default);
    Task<bool> IsbnExistsAsync(string isbn, Guid? excludeId, CancellationToken ct = default);
    Task<(List<Book> Items, int TotalCount)> SearchAsync(
        string? searchTerm, Guid? categoryId, Guid? authorId, Guid? branchId,
        int page, int pageSize, CancellationToken ct = default);
    Task AddAsync(Book book, CancellationToken ct = default);
    Task AddCopiesAsync(IEnumerable<BookCopy> copies, CancellationToken ct = default);
    Task UpdateAsync(Book book, CancellationToken ct = default);
    Task DeleteAsync(Book book, CancellationToken ct = default);
    Task<List<Author>> GetAllAuthorsAsync(CancellationToken ct = default);
    Task<List<Category>> GetAllCategoriesAsync(CancellationToken ct = default);
    Task AddAuthorAsync(Author author, CancellationToken ct = default);
    Task AddCategoryAsync(Category category, CancellationToken ct = default);
}
