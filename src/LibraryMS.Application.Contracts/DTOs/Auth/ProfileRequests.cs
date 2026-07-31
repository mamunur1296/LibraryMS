namespace LibraryMS.Application.Contracts.DTOs.Auth;

public class ChangePasswordRequest
{
    public string CurrentPassword { get; set; } = default!;
    public string NewPassword { get; set; } = default!;
}

public class ChangeUsernameRequest
{
    public string NewUsername { get; set; } = default!;
}

public class ChangeEmailRequest
{
    public string NewEmail { get; set; } = default!;
}
