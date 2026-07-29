using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.BookManagement.AggregateRoots;
using LibraryMS.Domain.BookManagement.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryMS.EntityFrameworkCore.Repositories;

public sealed class BookRepository : BaseRepository<Book>, IBookRepository
{
    public BookRepository(LibraryDbContext dbContext) : base(dbContext) { }

    public async Task<Book?> GetByIdWithCopiesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(b => b.Copies)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<Book?> GetByIsbnAsync(string isbn, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(b => b.Copies)
            .FirstOrDefaultAsync(b => b.ISBN.Value == isbn, cancellationToken);
    }

    public async Task<(List<Book> Items, int TotalCount)> SearchAsync(
        string? searchTerm, Guid? categoryId, Guid? authorId, Guid? branchId,
        int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            query = query.Where(b => b.Title.ToLower().Contains(searchTerm) || b.ISBN.Value.Contains(searchTerm));
        }

        if (categoryId.HasValue)
            query = query.Where(b => b.CategoryId == categoryId.Value);

        if (authorId.HasValue)
            query = query.Where(b => b.AuthorId == authorId.Value);

        if (branchId.HasValue)
            query = query.Where(b => b.Copies.Any(c => c.BranchId == branchId.Value));

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(b => b.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }
    public async Task<bool> IsbnExistsAsync(string isbn, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = DbSet.Where(b => b.ISBN.Value == isbn);
        if (excludeId.HasValue)
            query = query.Where(b => b.Id != excludeId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<List<Author>> GetAllAuthorsAsync(CancellationToken ct = default)
        => await DbContext.Authors.AsNoTracking().OrderBy(a => a.Name).ToListAsync(ct);

    public async Task<List<Category>> GetAllCategoriesAsync(CancellationToken ct = default)
        => await DbContext.Categories.AsNoTracking().OrderBy(c => c.Name).ToListAsync(ct);

    public async Task AddAuthorAsync(Author author, CancellationToken ct = default)
    {
        DbContext.Authors.Add(author);
        await DbContext.SaveChangesAsync(ct);
    }

    public async Task AddCategoryAsync(Category category, CancellationToken ct = default)
    {
        DbContext.Categories.Add(category);
        await DbContext.SaveChangesAsync(ct);
    }
}

