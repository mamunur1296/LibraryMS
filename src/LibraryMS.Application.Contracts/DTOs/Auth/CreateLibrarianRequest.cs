namespace LibraryMS.Application.Contracts.DTOs.Auth;

public class CreateLibrarianRequest
{
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public Guid? BranchId { get; set; }
}
