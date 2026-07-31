namespace LibraryMS.Application.Contracts.DTOs.Report;

public class MemberGrowthDto
{
    public int Month { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public int NewMembers { get; set; }
}
