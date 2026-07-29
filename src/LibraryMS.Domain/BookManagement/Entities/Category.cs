using LibraryMS.Domain.Common;

namespace LibraryMS.Domain.BookManagement;

/// <summary>Category entity for book classification.</summary>
public sealed class Category : Entity<Guid>
{
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }

    private Category() { }

    public Category(Guid id, string name, string? description = null)
        : base(id)
    {
        Name = name;
        Description = description;
        CreatedAt = DateTime.UtcNow;
    }
}

