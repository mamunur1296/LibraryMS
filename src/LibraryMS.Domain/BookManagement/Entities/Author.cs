using LibraryMS.Domain.Common;

namespace LibraryMS.Domain.BookManagement.Entities;

// Author entity — belongs to Book aggregate via navigation.
public sealed class Author : Entity<Guid>
{
    public string Name { get; private set; } = default!;
    public string? Biography { get; private set; }

    private Author() { }

    public Author(Guid id, string name, string? biography = null)
        : base(id)
    {
        Name = name;
        Biography = biography;
        CreatedAt = DateTime.UtcNow;
    }
}

