using LibraryMS.Application.Contracts.DTOs.Report;
using LibraryMS.Application.Contracts.Reports;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryMS.Application.Reports;

public class GetAnnualRevenueReportQueryHandler : IRequestHandler<GetAnnualRevenueReportQuery, List<AnnualRevenueDto>>
{
    private readonly IReportRepository _reportRepository;

    public GetAnnualRevenueReportQueryHandler(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task<List<AnnualRevenueDto>> Handle(GetAnnualRevenueReportQuery request, CancellationToken cancellationToken)
    {
        return await _reportRepository.GetAnnualRevenueAsync(request.Year, cancellationToken);
    }
}
