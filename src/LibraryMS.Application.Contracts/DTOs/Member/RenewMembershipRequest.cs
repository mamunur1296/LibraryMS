namespace LibraryMS.Application.Contracts.DTOs.Member;

public class RenewMembershipRequest
{
    public int Days { get; set; } = 365; // Default 1 year
}
