using System;

namespace LibraryMS.Application.Contracts.DTOs.Member;

public class MemberProfileStatsDto
{
    public Guid MemberId { get; set; }
    public int TotalBorrows { get; set; }
    public int ActiveBorrows { get; set; }
    public int OverdueBorrows { get; set; }
    public int ActiveReservations { get; set; }
    public decimal TotalFinesDue { get; set; }
}
