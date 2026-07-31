using LibraryMS.Application.Contracts.Books;
using LibraryMS.Application.Contracts.Common;
using LibraryMS.Application.Contracts.DTOs.Book;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryMS.HttpApi.Controllers;

[AllowAnonymous]
[Route("api/v{version:apiVersion}/public")]
public class PublicController : BaseController
{
    [HttpGet("books")]
    public async Task<ActionResult<PagedResult<BookDto>>> GetBooks(
        [FromQuery] string? searchTerm, [FromQuery] Guid? categoryId, [FromQuery] Guid? authorId, 
        [FromQuery] Guid? branchId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, 
        CancellationToken cancellationToken = default)
    {
        var query = new SearchBooksQuery(searchTerm, categoryId, authorId, branchId, page, pageSize);
        var result = await Mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("books/{id}")]
    public async Task<ActionResult<BookDto>> GetBook(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetBookByIdQuery(id), cancellationToken);
        if (result is null) return NotFound();
        return Ok(result);
    }
}
