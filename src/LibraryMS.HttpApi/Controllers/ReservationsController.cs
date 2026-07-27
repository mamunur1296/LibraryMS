using LibraryMS.Application.Contracts.Common;
using LibraryMS.Application.Contracts.DTOs.Reservation;
using LibraryMS.Application.Contracts.Reservations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryMS.HttpApi.Controllers;

[Authorize]
public class ReservationsController : BaseController
{
    [HttpGet]
    [Authorize(Roles = "Admin,Librarian")]
    public async Task<ActionResult<PagedResult<ReservationDto>>> GetAll(
        [FromQuery] Guid? memberId, [FromQuery] Guid? bookId, [FromQuery] string? status, 
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10, 
        CancellationToken cancellationToken = default)
    {
        var result = await Mediator.Send(new GetReservationsQuery(memberId, bookId, status, page, pageSize), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReservationDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetReservationByIdQuery(id), cancellationToken);
        if (result is null) return NotFound();
        
        if (User.IsInRole("Member") && result.MemberId.ToString() != User.Claims.FirstOrDefault(c => c.Type == "memberId")?.Value)
            return Forbid();
            
        return Ok(result);
    }

    [HttpGet("my-reservations")]
    [Authorize(Roles = "Member")]
    public async Task<ActionResult<List<ReservationDto>>> GetMyReservations(CancellationToken cancellationToken)
    {
        var memberIdString = User.Claims.FirstOrDefault(c => c.Type == "memberId")?.Value;
        if (!Guid.TryParse(memberIdString, out var memberId)) return Unauthorized();

        var result = await Mediator.Send(new GetMemberReservationsQuery(memberId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("queue")]
    [AllowAnonymous]
    public async Task<ActionResult<ReservationQueueDto>> GetQueue([FromQuery] Guid bookId, [FromQuery] Guid branchId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetBookQueueQuery(bookId, branchId), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ReservationDto>> Create([FromBody] CreateReservationCommand command, CancellationToken cancellationToken)
    {
        if (User.IsInRole("Member") && command.MemberId.ToString() != User.Claims.FirstOrDefault(c => c.Type == "memberId")?.Value)
            return Forbid();
            
        var result = await Mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        var requestingMemberId = Guid.Empty;
        if (User.IsInRole("Member"))
        {
            var memberIdString = User.Claims.FirstOrDefault(c => c.Type == "memberId")?.Value;
            Guid.TryParse(memberIdString, out requestingMemberId);
        }

        await Mediator.Send(new CancelReservationCommand(id, requestingMemberId), cancellationToken);
        return NoContent();
    }
}
