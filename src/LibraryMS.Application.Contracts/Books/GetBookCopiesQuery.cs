using LibraryMS.Application.Contracts.DTOs.Book;
using MediatR;
using System;
using System.Collections.Generic;

namespace LibraryMS.Application.Contracts.Books;

public sealed record GetBookCopiesQuery(Guid BookId) : IRequest<List<BookCopyDto>>;
