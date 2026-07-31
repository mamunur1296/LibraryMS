using LibraryMS.Application.Contracts.DTOs.Report;
using LibraryMS.Application.Contracts.Reports;
using MediatR;

namespace LibraryMS.Application.Reports;

public class GetBranchComparisonReportQueryHandler : IRequestHandler<GetBranchComparisonReportQuery, List<BranchComparisonDto>>
{
    private readonly IReportRepository _reportRepository;

    public GetBranchComparisonReportQueryHandler(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task<List<BranchComparisonDto>> Handle(GetBranchComparisonReportQuery request, CancellationToken cancellationToken)
    {
        return await _reportRepository.GetBranchComparisonAsync(cancellationToken);
    }
}
