using LibraryMS.Application.Contracts.Services;
using LibraryMS.Domain.BorrowManagement;
using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.Shared;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.BackgroundJobs;

public sealed class DueDateReminderJob
{
    private readonly IBorrowRepository _borrowRepository;
    private readonly ILogger<DueDateReminderJob> _logger;

    private readonly IEmailService _emailService;
    private readonly IMemberRepository _memberRepository;

    public DueDateReminderJob(
        IBorrowRepository borrowRepository,
        IEmailService emailService,
        IMemberRepository memberRepository,
        ILogger<DueDateReminderJob> logger)
    {
        _borrowRepository = borrowRepository;
        _emailService = emailService;
        _memberRepository = memberRepository;
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting DueDateReminderJob...");

        var targetDate = DateTime.UtcNow.Date.AddDays(1);
        
        // Find all active borrows due tomorrow
        // We fetch active borrows and filter in-memory since the repository doesn't have a specific due date filter
        var activeBorrows = await _borrowRepository.GetPagedAsync(null, null, "Active", 1, 10000, cancellationToken);
        var dueTomorrow = activeBorrows.Items.Where(b => b.DueDate.Date == targetDate).ToList();

        foreach (var borrow in dueTomorrow)
        {
            var member = await _memberRepository.GetByIdAsync(borrow.MemberId, cancellationToken);
            if (member == null) continue;

            _logger.LogInformation("Sending reminder for Borrow ID {BorrowId} due on {DueDate}", borrow.Id, borrow.DueDate);
            await _emailService.SendAsync(
                member.Email,
                $"{member.FirstName} {member.LastName}",
                "Book Due Reminder",
                $"Dear {member.FirstName}, your borrowed book is due tomorrow.",
                cancellationToken);
        }

        _logger.LogInformation("Finished DueDateReminderJob. Sent {Count} reminders.", dueTomorrow.Count);
    }
}
