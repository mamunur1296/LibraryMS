namespace LibraryMS.Application.Contracts.Services;

// Adapter Pattern interface for Email sending.
// Application layer depends on this abstraction, not on MailKit directly.
public interface IEmailService
{
    Task SendAsync(string toEmail, string toName, string subject, string htmlBody, CancellationToken cancellationToken = default);
}
