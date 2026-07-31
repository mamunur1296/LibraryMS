using LibraryMS.Application.Contracts.Common;
using LibraryMS.Application.Contracts.DTOs.Member;
using LibraryMS.Application.Contracts.DTOs.Borrow;
using LibraryMS.Application.Contracts.Members;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryMS.HttpApi.Controllers;

[Authorize]
public class MembersController : BaseController
{
    [HttpGet]
    [Authorize(Roles = "Admin,Librarian")]
    public async Task<ActionResult<PagedResult<MemberDto>>> Search(
        [FromQuery] string? searchTerm, [FromQuery] string? status, 
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10, 
        CancellationToken cancellationToken = default)
    {
        var result = await Mediator.Send(new SearchMembersQuery(searchTerm, status, page, pageSize), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Librarian")]
    public async Task<ActionResult<MemberDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetMemberByIdQuery(id), cancellationToken);
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpGet("{id}/stats")]
    [Authorize(Roles = "Admin,Librarian,Member")]
    public async Task<ActionResult<MemberProfileStatsDto>> GetStats(Guid id, CancellationToken cancellationToken)
    {
        if (!IsAuthorizedForMember(id)) return Forbid();
        
        var result = await Mediator.Send(new GetMemberProfileStatsQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}/active-borrows")]
    [Authorize(Roles = "Admin,Librarian,Member")]
    public async Task<ActionResult<List<BorrowDto>>> GetActiveBorrows(Guid id, CancellationToken cancellationToken)
    {
        if (!IsAuthorizedForMember(id)) return Forbid();

        var result = await Mediator.Send(new GetMemberActiveBorrowsQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}/fines")]
    [Authorize(Roles = "Admin,Librarian,Member")]
    public async Task<ActionResult<List<BorrowDto>>> GetFineHistory(Guid id, CancellationToken cancellationToken)
    {
        if (!IsAuthorizedForMember(id)) return Forbid();

        var result = await Mediator.Send(new GetMemberFineHistoryQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Librarian")]
    public async Task<ActionResult<MemberDto>> Create([FromBody] CreateMemberCommand command, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Librarian")]
    public async Task<ActionResult<MemberDto>> Update(Guid id, [FromBody] UpdateMemberCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id) return BadRequest();
        var result = await Mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await Mediator.Send(new DeleteMemberCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpPost("{id}/suspend")]
    [Authorize(Roles = "Admin,Librarian")]
    public async Task<ActionResult<MemberDto>> Suspend(Guid id, [FromBody] SuspendMemberCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id) return BadRequest();
        var result = await Mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id}/activate")]
    [Authorize(Roles = "Admin,Librarian")]
    public async Task<ActionResult<MemberDto>> Activate(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new ActivateMemberCommand(id), cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id}/reset-password")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ResetPassword(Guid id, [FromBody] ResetMemberPasswordRequest request, CancellationToken cancellationToken)
    {
        await Mediator.Send(new ResetMemberPasswordCommand(id, request.NewPassword), cancellationToken);
        return Ok(new { Message = "Password reset successfully." });
    }

    [HttpPost("{id}/create-account")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateAccount(Guid id, [FromBody] CreateMemberUserRequest request, CancellationToken cancellationToken)
    {
        await Mediator.Send(new CreateMemberUserCommand(id, request.Username, request.Password), cancellationToken);
        return Ok(new { Message = "Account created successfully." });
    }

    [HttpPost("{id}/renew")]
    [Authorize(Roles = "Admin,Librarian,Member")]
    public async Task<ActionResult<MemberDto>> Renew(Guid id, [FromBody] RenewMembershipRequest request, CancellationToken cancellationToken)
    {
        if (!IsAuthorizedForMember(id)) return Forbid();

        var result = await Mediator.Send(new RenewMembershipCommand(id, request.Days), cancellationToken);
        return Ok(result);
    }

    private bool IsAuthorizedForMember(Guid requestedMemberId)
    {
        if (User.IsInRole("Admin") || User.IsInRole("Librarian"))
            return true;

        if (User.IsInRole("Member"))
        {
            var memberIdString = User.Claims.FirstOrDefault(c => c.Type.Equals("memberId", StringComparison.OrdinalIgnoreCase))?.Value;
            if (memberIdString != null && Guid.TryParse(memberIdString, out var currentMemberId))
            {
                return currentMemberId == requestedMemberId;
            }
        }
        return false;
    }
}
