using LibraryMS.Application.Contracts.DTOs.Auth;
using LibraryMS.Application.Contracts.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryMS.HttpApi.Controllers;

public class UsersController : BaseController
{
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserDto>> GetCurrentUser(CancellationToken cancellationToken)
    {
        var userIdString = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value
                           ?? User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdString, out var userId))
            return Unauthorized();

        var user = await Mediator.Send(new GetCurrentUserQuery(userId), cancellationToken);
        if (user is null) return NotFound();

        return Ok(user);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<UserDto>>> GetAllUsers(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetAllUsersQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        var userIdString = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value
                           ?? User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdString, out var userId) || command.UserId != userId)
            return Forbid();

        await Mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPost("change-username")]
    [Authorize]
    public async Task<IActionResult> ChangeUsername([FromBody] ChangeUsernameCommand command, CancellationToken cancellationToken)
    {
        var userIdString = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value
                           ?? User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdString, out var userId) || command.UserId != userId)
            return Forbid();

        await Mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPost("change-email")]
    [Authorize]
    public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailCommand command, CancellationToken cancellationToken)
    {
        var userIdString = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value
                           ?? User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdString, out var userId) || command.UserId != userId)
            return Forbid();

        await Mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPost("change-role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ChangeRole([FromBody] ChangeUserRoleCommand command, CancellationToken cancellationToken)
    {
        await Mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id}/suspend")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Suspend(Guid id, CancellationToken cancellationToken)
    {
        await Mediator.Send(new SuspendUserCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpPost("{id}/activate")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Activate(Guid id, CancellationToken cancellationToken)
    {
        await Mediator.Send(new ActivateUserCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpPost("{id}/assign-branch")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignBranch(Guid id, [FromBody] AssignBranchRequest request, CancellationToken cancellationToken)
    {
        await Mediator.Send(new AssignBranchToLibrarianCommand(id, request.BranchId), cancellationToken);
        return NoContent();
    }

    [HttpPost("create-librarian")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Guid>> CreateLibrarian([FromBody] CreateLibrarianCommand command, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}

public class AssignBranchRequest
{
    public Guid BranchId { get; set; }
}
