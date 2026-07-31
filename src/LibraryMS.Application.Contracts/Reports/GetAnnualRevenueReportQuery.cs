using LibraryMS.Application.Contracts.DTOs.Report;
using MediatR;
using System.Collections.Generic;

namespace LibraryMS.Application.Contracts.Reports;

public class GetAnnualRevenueReportQuery : IRequest<List<AnnualRevenueDto>>
{
    public int Year { get; set; }
    
    public GetAnnualRevenueReportQuery(int year)
    {
        Year = year;
    }
}
