using LibraryMS.Application.Contracts.Borrows;
using LibraryMS.Application.Contracts.Common;
using LibraryMS.Application.Contracts.DTOs.Borrow;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryMS.HttpApi.Controllers;

[Authorize]
public class BorrowsController : BaseController
{
    [HttpGet]
    [Authorize(Roles = "Admin,Librarian")]
    public async Task<ActionResult<PagedResult<BorrowDto>>> GetAll(
        [FromQuery] Guid? memberId, [FromQuery] Guid? bookId, [FromQuery] string? status, 
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10, 
        CancellationToken cancellationToken = default)
    {
        var result = await Mediator.Send(new GetBorrowsQuery(memberId, bookId, status, page, pageSize), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BorrowDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetBorrowByIdQuery(id), cancellationToken);
        if (result is null) return NotFound();
        
        // Members can only view their own borrows
        if (User.IsInRole("Member") && result.MemberId.ToString() != User.Claims.FirstOrDefault(c => c.Type.Equals("memberId", StringComparison.OrdinalIgnoreCase))?.Value)
            return Forbid();
            
        return Ok(result);
    }

    [HttpGet("my-active")]
    [Authorize(Roles = "Member")]
    public async Task<ActionResult<List<BorrowDto>>> GetMyActiveBorrows(CancellationToken cancellationToken)
    {
        var memberIdString = User.Claims.FirstOrDefault(c => c.Type.Equals("memberId", StringComparison.OrdinalIgnoreCase))?.Value;
        if (!Guid.TryParse(memberIdString, out var memberId)) return Unauthorized();

        var result = await Mediator.Send(new GetActiveBorrowsByMemberQuery(memberId), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Librarian")]
    public async Task<ActionResult<BorrowDto>> Borrow([FromBody] BorrowBookCommand command, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPost("return")]
    [Authorize(Roles = "Librarian")]
    public async Task<ActionResult<BorrowDto>> Return([FromBody] ReturnBookCommand command, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id}/pay-fine")]
    [Authorize(Roles = "Librarian")]
    public async Task<ActionResult<BorrowDto>> PayFine(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new PayFineCommand(id), cancellationToken);
        return Ok(result);
    }
}
