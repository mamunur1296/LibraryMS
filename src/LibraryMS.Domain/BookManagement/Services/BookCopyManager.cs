using LibraryMS.Domain.Common;
using LibraryMS.Domain.BookManagement.AggregateRoots;
using LibraryMS.Domain.BookManagement.Entities;
using System;

namespace LibraryMS.Domain.BookManagement.Services;

public sealed class BookCopyManager
{
    private readonly IGuidGenerator _guidGenerator;

    public BookCopyManager(IGuidGenerator guidGenerator)
    {
        _guidGenerator = guidGenerator;
    }

    public BookCopy AddCopyToBranch(Book book, Guid branchId)
    {
        ArgumentNullException.ThrowIfNull(book);
        return book.AddCopy(_guidGenerator.Create(), branchId);
    }
}
