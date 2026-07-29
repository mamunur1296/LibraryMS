using LibraryMS.Domain.Common;
using System;

using LibraryMS.Domain.BookManagement.Entities;

namespace LibraryMS.Domain.BookManagement.Services;

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
