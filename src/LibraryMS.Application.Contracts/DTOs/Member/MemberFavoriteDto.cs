using System;
using LibraryMS.Application.Contracts.DTOs.Book;

namespace LibraryMS.Application.Contracts.DTOs.Member;

public class MemberFavoriteDto
{
    public Guid BookId { get; set; }
    public BookDto Book { get; set; } = default!;
}
