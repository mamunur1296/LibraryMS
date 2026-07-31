using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace LibraryMS.HttpApi.Controllers.V2;

[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/books")]
public class BooksController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            Message = "This is Books API Version 2.0 (Demo)",
            Timestamp = DateTime.UtcNow,
            Status = "Active",
            DeveloperNote = "Version 2.0 has custom fields and optimized payloads."
        });
    }
}
