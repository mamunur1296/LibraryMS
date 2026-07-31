namespace LibraryMS.Application.Contracts.DTOs.Member;

public sealed class SuspendMemberRequest
{
    public DateTime SuspendedUntil { get; init; }
    public string Reason { get; init; } = default!;
}
