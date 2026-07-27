using LibraryMS.Application.Contracts.Books;
using LibraryMS.Application.Contracts.Common;
using LibraryMS.Application.Contracts.DTOs.Book;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryMS.HttpApi.Controllers;

[Authorize(Roles = "Admin,Librarian")]
public class BooksController : BaseController
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResult<BookDto>>> Search(
        [FromQuery] string? searchTerm, [FromQuery] Guid? categoryId, [FromQuery] Guid? authorId, 
        [FromQuery] Guid? branchId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, 
        CancellationToken cancellationToken = default)
    {
        var query = new SearchBooksQuery(searchTerm, categoryId, authorId, branchId, page, pageSize);
        var result = await Mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<BookDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetBookByIdQuery(id), cancellationToken);
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<BookDto>> Create([FromBody] CreateBookCommand command, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<BookDto>> Update(Guid id, [FromBody] UpdateBookCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id) return BadRequest();
        var result = await Mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await Mediator.Send(new DeleteBookCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpPost("{id}/copies")]
    public async Task<ActionResult<List<BookCopyDto>>> AddCopies(Guid id, [FromBody] AddBookCopiesCommand command, CancellationToken cancellationToken)
    {
        if (id != command.BookId) return BadRequest();
        var result = await Mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}/available-copies")]
    public async Task<ActionResult<List<BookCopyDto>>> GetAvailableCopies(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetAvailableCopiesQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpGet("authors")]
    [AllowAnonymous]
    public async Task<ActionResult<List<AuthorDto>>> GetAllAuthors(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetAllAuthorsQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPost("authors")]
    public async Task<ActionResult<AuthorDto>> CreateAuthor([FromBody] CreateAuthorCommand command, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpGet("categories")]
    [AllowAnonymous]
    public async Task<ActionResult<List<CategoryDto>>> GetAllCategories(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetAllCategoriesQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPost("categories")]
    public async Task<ActionResult<CategoryDto>> CreateCategory([FromBody] CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}
