using LibraryMS.Application.Contracts.Members;
<<<<<<< Updated upstream
=======
using LibraryMS.Application.Contracts.DTOs.Member;
>>>>>>> Stashed changes
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryMS.HttpApi.Controllers;

[Authorize(Roles = "Member")]
[Route("api/favourites")]
public class FavouritesController : BaseController
{
<<<<<<< Updated upstream
    [HttpPost("{bookId}")]
    public async Task<IActionResult> AddFavorite(Guid bookId, CancellationToken cancellationToken)
    {
        var memberIdString = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value
                             ?? User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
=======
    [HttpGet]
    public async Task<ActionResult<List<MemberFavoriteDto>>> GetFavorites(CancellationToken cancellationToken)
    {
        var memberIdString = User.Claims.FirstOrDefault(c => c.Type == "MemberId")?.Value
                             ?? User.Claims.FirstOrDefault(c => c.Type == "memberId")?.Value;

        if (!Guid.TryParse(memberIdString, out var memberId))
            return Unauthorized();

        var result = await Mediator.Send(new GetMemberFavoritesQuery(memberId), cancellationToken);
        return Ok(result);
    }
    [HttpPost("{bookId}")]
    public async Task<IActionResult> AddFavorite(Guid bookId, CancellationToken cancellationToken)
    {
        var memberIdString = User.Claims.FirstOrDefault(c => c.Type == "MemberId")?.Value
                             ?? User.Claims.FirstOrDefault(c => c.Type == "memberId")?.Value;
>>>>>>> Stashed changes

        if (!Guid.TryParse(memberIdString, out var memberId))
            return Unauthorized();

        await Mediator.Send(new AddFavoriteCommand(memberId, bookId), cancellationToken);
        return Ok(new { Message = "Book added to favourites." });
    }

    [HttpDelete("{bookId}")]
    public async Task<IActionResult> RemoveFavorite(Guid bookId, CancellationToken cancellationToken)
    {
<<<<<<< Updated upstream
        var memberIdString = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value
                             ?? User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
=======
        var memberIdString = User.Claims.FirstOrDefault(c => c.Type == "MemberId")?.Value
                             ?? User.Claims.FirstOrDefault(c => c.Type == "memberId")?.Value;
>>>>>>> Stashed changes

        if (!Guid.TryParse(memberIdString, out var memberId))
            return Unauthorized();

        await Mediator.Send(new RemoveFavoriteCommand(memberId, bookId), cancellationToken);
        return Ok(new { Message = "Book removed from favourites." });
    }
}
