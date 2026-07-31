namespace LibraryMS.Application.Contracts.DTOs.Report;

public class AnnualRevenueDto
{
    public int Month { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
}
