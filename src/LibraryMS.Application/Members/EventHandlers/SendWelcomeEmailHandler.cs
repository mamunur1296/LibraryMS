using LibraryMS.Application.Contracts.Services;
using LibraryMS.Domain.MemberManagement.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.Members.EventHandlers;

public sealed class SendWelcomeEmailHandler : INotificationHandler<MemberRegisteredEvent>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<SendWelcomeEmailHandler> _logger;

    public SendWelcomeEmailHandler(
        IEmailService emailService,
        ILogger<SendWelcomeEmailHandler> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Handle(MemberRegisteredEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending welcome email to new member: {Email}", notification.Email);

        var fullName = $"{notification.FirstName} {notification.LastName}";
        var subject = "Welcome to LibraryMS!";
        
        var body = $@"
            <html>
            <body>
                <h2>Welcome to LibraryMS, {notification.FirstName}!</h2>
                <p>We are very excited to have you as a new member.</p>
                <p>You can now browse our catalog and borrow books.</p>
                <br/>
                <p>Best regards,<br/>The LibraryMS Team</p>
            </body>
            </html>";

        await _emailService.SendAsync(notification.Email, fullName, subject, body, cancellationToken);
        
        _logger.LogInformation("Welcome email sent to {Email} successfully.", notification.Email);
    }
}
