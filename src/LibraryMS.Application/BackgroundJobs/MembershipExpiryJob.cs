using LibraryMS.Application.Contracts.Services;
using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.Shared;
using LibraryMS.Domain.Shared.Enums;
using Microsoft.Extensions.Logging;

namespace LibraryMS.Application.BackgroundJobs;

public sealed class MembershipExpiryJob
{
    private readonly IMemberRepository _memberRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly ILogger<MembershipExpiryJob> _logger;

    public MembershipExpiryJob(
        IMemberRepository memberRepository,
        IUnitOfWork unitOfWork,
        IEmailService emailService,
        ILogger<MembershipExpiryJob> logger)
    {
        _memberRepository = memberRepository;
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting MembershipExpiryJob...");

        // Fetch members whose expiry date has passed and are still active
        var activeMembers = await _memberRepository.SearchAsync(
            searchTerm: null,
            status: "Active",
            page: 1,
            pageSize: 10000);

        var expiredMembersCount = 0;
        foreach (var member in activeMembers.Items)
        {
            if (member.MembershipExpiry <= DateTime.UtcNow)
            {
                member.Suspend(DateTime.UtcNow, "Membership expired.");
                await _memberRepository.UpdateAsync(member, cancellationToken);
                expiredMembersCount++;

                await _emailService.SendAsync(
                    member.Email,
                    $"{member.FirstName} {member.LastName}",
                    "Membership Expired",
                    $"Dear {member.FirstName}, your library membership has expired and your account has been suspended. Please renew your membership to continue borrowing books.",
                    cancellationToken);
            }
        }

        if (expiredMembersCount > 0)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        _logger.LogInformation("Finished MembershipExpiryJob. Suspended {Count} expired members.", expiredMembersCount);
    }
}
