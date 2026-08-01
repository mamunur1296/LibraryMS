namespace LibraryMS.Infrastructure.Email;

public sealed class SmtpOptions
{
    public const string SectionName = "Smtp";
    public string Host { get; init; } = default!;
    public int Port { get; init; } = 587;
    public string Username { get; init; } = default!;
    public string Password { get; init; } = default!;
    public string FromEmail { get; init; } = default!;
    public string FromName { get; init; } = "LibraryMS";
    public bool EnableSsl { get; init; } = true;
    public bool IsEnabled { get; init; } = true;
}
