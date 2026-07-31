namespace LibraryMS.Application.Contracts.DTOs.Member;

public class CreateMemberUserRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
