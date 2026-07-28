using LibraryMS.Application.Contracts.Auth;
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
}
