using LibraryMS.Application.Contracts.DTOs.Report;
using MediatR;
using System.Collections.Generic;

namespace LibraryMS.Application.Contracts.Reports;

public class GetMemberGrowthReportQuery : IRequest<List<MemberGrowthDto>>
{
    public int Year { get; set; }
    
    public GetMemberGrowthReportQuery(int year)
    {
        Year = year;
    }
}
