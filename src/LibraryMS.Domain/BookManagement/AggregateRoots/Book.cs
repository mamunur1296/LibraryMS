using LibraryMS.Domain.BookManagement.Entities;
using LibraryMS.Domain.BookManagement.Events;
using LibraryMS.Domain.BookManagement.VOs;
using LibraryMS.Domain.Common;
using LibraryMS.Domain.Shared.Guards;
using LibraryMS.Domain.Shared.Constants;

namespace LibraryMS.Domain.BookManagement.AggregateRoots;

// Book — Aggregate Root for the Book Management bounded context.
// Owns BookCopy entities. Controls copy lifecycle.
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
        Ensure.Against(url != null && url.Length > BookConsts.MaxCoverImageUrlLength, $"Cover image URL cannot exceed {BookConsts.MaxCoverImageUrlLength} characters.", "BOOK_COVER_IMAGE_URL_TOO_LONG");
        CoverImageUrl = url;
        LastModifiedAt = DateTime.UtcNow;
    }

    // Adds a new physical copy of this book to a branch.
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

    // Marks a specific copy as borrowed — called by BorrowManager.
    internal BookCopy BorrowCopy(Guid copyId)
    {
        var copy = GetCopyOrThrow(copyId);
        copy.MarkAsBorrowed();
        return copy;
    }

    // Marks a specific copy as returned (available).
    internal BookCopy ReturnCopy(Guid copyId)
    {
        var copy = GetCopyOrThrow(copyId);
        copy.MarkAsAvailable();
        return copy;
    }

    // Returns the first available copy in a specific branch.
    internal BookCopy? GetAvailableCopyInBranch(Guid branchId)
        => _copies.FirstOrDefault(c => c.BranchId == branchId && c.IsAvailable);

    public int TotalCopies => _copies.Count;
    public int AvailableCopies => _copies.Count(c => c.IsAvailable);

    private BookCopy GetCopyOrThrow(Guid copyId)
    {
        var copy = _copies.FirstOrDefault(c => c.Id == copyId);
        Ensure.Found(copy, nameof(BookCopy), copyId);
        return copy!;
    }

    private string GenerateCopyNumber(Guid branchId)
    {
        var count = _copies.Count(c => c.BranchId == branchId) + 1;
        return $"B{branchId.ToString()[..4].ToUpper()}-C{count:D3}";
    }

    private void SetTitle(string title)
    {
        Ensure.Against(string.IsNullOrWhiteSpace(title), "Book title cannot be empty.", "BOOK_TITLE_EMPTY");
        Ensure.Against(title.Length > BookConsts.MaxTitleLength, $"Book title cannot exceed {BookConsts.MaxTitleLength} characters.", "BOOK_TITLE_TOO_LONG");
        Title = title.Trim();
    }

    private void SetPublicationYear(int year)
    {
        Ensure.Against(year < 1000 || year > DateTime.UtcNow.Year + 1, $"Invalid publication year: {year}.", "BOOK_INVALID_YEAR");
        PublicationYear = year;
    }
}


