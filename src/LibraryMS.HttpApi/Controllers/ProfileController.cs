using LibraryMS.Application.Contracts.DTOs.Auth;
using LibraryMS.Application.Contracts.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryMS.HttpApi.Controllers;

[Authorize]
[Route("api/v{version:apiVersion}/profile")]
public class ProfileController : BaseController
{
    [HttpGet]
    public async Task<ActionResult<UserDto>> GetMyProfile(CancellationToken cancellationToken)
    {
        var userIdString = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value
                           ?? User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdString, out var userId))
            return Unauthorized();

        var user = await Mediator.Send(new GetCurrentUserQuery(userId), cancellationToken);
        if (user is null) return NotFound();

        return Ok(user);
    }

    [HttpPut("password")]
    public async Task<IActionResult> UpdatePassword([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var userIdString = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value
                           ?? User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdString, out var userId))
            return Unauthorized();

        await Mediator.Send(new ChangePasswordCommand(userId, request.CurrentPassword, request.NewPassword), cancellationToken);
        return Ok(new { Message = "Password updated successfully." });
    }

    [HttpPut("username")]
    public async Task<IActionResult> UpdateUsername([FromBody] ChangeUsernameRequest request, CancellationToken cancellationToken)
    {
        var userIdString = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value
                           ?? User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdString, out var userId))
            return Unauthorized();

        await Mediator.Send(new ChangeUsernameCommand(userId, request.NewUsername), cancellationToken);
        return Ok(new { Message = "Username updated successfully." });
    }

    [HttpPut("email")]
    public async Task<IActionResult> UpdateEmail([FromBody] ChangeEmailRequest request, CancellationToken cancellationToken)
    {
        var userIdString = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value
                           ?? User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdString, out var userId))
            return Unauthorized();

        await Mediator.Send(new ChangeEmailCommand(userId, request.NewEmail), cancellationToken);
        return Ok(new { Message = "Email updated successfully." });
    }
}
