using LibraryMS.Application.Contracts.Settings;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryMS.HttpApi.Controllers;

[Authorize(Roles = "Admin")]
public class SettingsController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetSettings()
    {
        var result = await Mediator.Send(new GetSettingsQuery());
        return Ok(result);
    }

    [HttpPut("{key}")]
    public async Task<IActionResult> UpdateSetting(string key, [FromBody] UpdateSettingRequest request)
    {
        await Mediator.Send(new UpdateSettingCommand(key, request.Value));
        return NoContent();
    }
}
