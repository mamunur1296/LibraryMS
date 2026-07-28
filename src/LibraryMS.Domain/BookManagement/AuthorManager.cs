using LibraryMS.Domain.Common;
using System;

namespace LibraryMS.Domain.BookManagement;

public sealed class AuthorManager
{
    private readonly IGuidGenerator _guidGenerator;

    public AuthorManager(IGuidGenerator guidGenerator)
    {
        _guidGenerator = guidGenerator;
    }

    public Author Create(string name, string? biography = null)
    {
        return new Author(_guidGenerator.Create(), name, biography);
    }
}
