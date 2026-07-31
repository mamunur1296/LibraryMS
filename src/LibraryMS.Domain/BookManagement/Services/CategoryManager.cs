using LibraryMS.Domain.Common;
using LibraryMS.Domain.BookManagement.Entities;
using System;

namespace LibraryMS.Domain.BookManagement.Services;

public sealed class CategoryManager
{
    private readonly IGuidGenerator _guidGenerator;

    public CategoryManager(IGuidGenerator guidGenerator)
    {
        _guidGenerator = guidGenerator;
    }

    public Category Create(string name, string? description = null)
    {
        return new Category(_guidGenerator.Create(), name, description);
    }
}
