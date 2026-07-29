using LibraryMS.Application.Contracts.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

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
}

// Adapter Pattern: Wraps MailKit into our IEmailService abstraction.
// Application layer never imports MailKit directly.
public sealed class MailKitEmailService : IEmailService
{
    private readonly SmtpOptions _options;

    public MailKitEmailService(IOptions<SmtpOptions> options)
        => _options = options.Value;

    public async Task SendAsync(
        string toEmail, string toName, string subject, string htmlBody,
        CancellationToken cancellationToken = default)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_options.FromName, _options.FromEmail));
        message.To.Add(new MailboxAddress(toName, toEmail));
        message.Subject = subject;

        var builder = new BodyBuilder { HtmlBody = htmlBody };
        message.Body = builder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_options.Host, _options.Port,
            _options.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None,
            cancellationToken);

        await client.AuthenticateAsync(_options.Username, _options.Password, cancellationToken);
        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(quit: true, cancellationToken);
    }
}
