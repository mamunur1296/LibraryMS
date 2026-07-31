using LibraryMS.Application.Contracts.DTOs.Auth;
using LibraryMS.Application.Contracts.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryMS.HttpApi.Controllers;

[Authorize(Roles = "Admin")]
public class LibrariansController : BaseController
{
    [HttpPost]
    public async Task<IActionResult> CreateLibrarian([FromBody] CreateLibrarianRequest request, CancellationToken cancellationToken)
    {
        var librarianId = await Mediator.Send(
            new CreateLibrarianCommand(request.Username, request.Email, request.Password, request.BranchId),
            cancellationToken);

        return Ok(new { Id = librarianId, Message = "Librarian created successfully." });
    }

    [HttpPost("{id}/assign-branch")]
    public async Task<IActionResult> AssignBranch(Guid id, [FromBody] Guid branchId, CancellationToken cancellationToken)
    {
        await Mediator.Send(new AssignLibrarianToBranchCommand(id, branchId), cancellationToken);
        return Ok(new { Message = "Branch assigned successfully." });
    }
}
