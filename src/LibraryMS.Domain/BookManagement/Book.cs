using LibraryMS.Domain.Common;
using LibraryMS.Domain.BookManagement.Events;
using LibraryMS.Domain.BookManagement.VOs;
using LibraryMS.Domain.Shared.Exceptions;

namespace LibraryMS.Domain.BookManagement;

/// <summary>
/// Book — Aggregate Root for the Book Management bounded context.
/// Owns BookCopy entities. Controls copy lifecycle.
/// </summary>
public sealed class Book : AggregateRoot<Guid>
{
    public string Title { get; private set; } = default!;
    public ISBN ISBN { get; private set; } = default!;
    public string? Description { get; private set; }
    public int PublicationYear { get; private set; }
    public Guid CategoryId { get; private set; }
    public Guid AuthorId { get; private set; }
    public string Language { get; private set; } = "English";
    public string? CoverImageUrl { get; private set; }

    // Optimistic concurrency token
    public byte[] RowVersion { get; private set; } = default!;

    private readonly List<BookCopy> _copies = [];
    public IReadOnlyList<BookCopy> Copies => _copies.AsReadOnly();

    private Book() { }

    internal Book(Guid id, string title, string isbn, string? description,
        int publicationYear, Guid categoryId, Guid authorId, string language)
        : base(id)
    {
        SetTitle(title);
        ISBN = ISBN.Create(isbn);
        Description = description;
        SetPublicationYear(publicationYear);
        CategoryId = categoryId;
        AuthorId = authorId;
        Language = language;
        CreatedAt = DateTime.UtcNow;

        AddDomainEvent(new BookCreatedEvent(id, title, isbn));
    }

    internal void Update(string title, string? description, int publicationYear,
        Guid categoryId, Guid authorId, string language)
    {
        SetTitle(title);
        Description = description;
        SetPublicationYear(publicationYear);
        CategoryId = categoryId;
        AuthorId = authorId;
        Language = language;
        LastModifiedAt = DateTime.UtcNow;
    }

    internal void SetCoverImage(string? url)
    {
        CoverImageUrl = url;
        LastModifiedAt = DateTime.UtcNow;
    }

    /// <summary>Adds a new physical copy of this book to a branch.</summary>
    internal BookCopy AddCopy(Guid branchId)
    {
        return AddCopy(Guid.NewGuid(), branchId);
    }

    internal BookCopy AddCopy(Guid copyId, Guid branchId)
    {
        var copyNumber = GenerateCopyNumber(branchId);
        var copy = new BookCopy(copyId, Id, branchId, copyNumber);
        _copies.Add(copy);
        AddDomainEvent(new BookCopyAddedEvent(Id, copy.Id, branchId));
        return copy;
    }

    /// <summary>Marks a specific copy as borrowed — called by BorrowManager.</summary>
    internal BookCopy BorrowCopy(Guid copyId)
    {
        var copy = GetCopyOrThrow(copyId);
        copy.MarkAsBorrowed();
        return copy;
    }

    /// <summary>Marks a specific copy as returned (available).</summary>
    internal BookCopy ReturnCopy(Guid copyId)
    {
        var copy = GetCopyOrThrow(copyId);
        copy.MarkAsAvailable();
        return copy;
    }

    /// <summary>Returns the first available copy in a specific branch.</summary>
    internal BookCopy? GetAvailableCopyInBranch(Guid branchId)
        => _copies.FirstOrDefault(c => c.BranchId == branchId && c.IsAvailable);

    public int TotalCopies => _copies.Count;
    public int AvailableCopies => _copies.Count(c => c.IsAvailable);

    private BookCopy GetCopyOrThrow(Guid copyId)
        => _copies.FirstOrDefault(c => c.Id == copyId)
           ?? throw new NotFoundException(nameof(BookCopy), copyId);

    private string GenerateCopyNumber(Guid branchId)
    {
        var count = _copies.Count(c => c.BranchId == branchId) + 1;
        return $"B{branchId.ToString()[..4].ToUpper()}-C{count:D3}";
    }

    private void SetTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Book title cannot be empty.", "BOOK_TITLE_EMPTY");
        if (title.Length > 300)
            throw new DomainException("Book title cannot exceed 300 characters.", "BOOK_TITLE_TOO_LONG");
        Title = title.Trim();
    }

    private void SetPublicationYear(int year)
    {
        if (year < 1000 || year > DateTime.UtcNow.Year + 1)
            throw new DomainException($"Invalid publication year: {year}.", "BOOK_INVALID_YEAR");
        PublicationYear = year;
    }
}


