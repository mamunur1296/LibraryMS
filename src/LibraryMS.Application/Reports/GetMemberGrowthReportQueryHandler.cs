using LibraryMS.Application.Contracts.DTOs.Report;
using LibraryMS.Application.Contracts.Reports;
using MediatR;

namespace LibraryMS.Application.Reports;

public class GetMemberGrowthReportQueryHandler : IRequestHandler<GetMemberGrowthReportQuery, List<MemberGrowthDto>>
{
    private readonly IReportRepository _reportRepository;

    public GetMemberGrowthReportQueryHandler(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task<List<MemberGrowthDto>> Handle(GetMemberGrowthReportQuery request, CancellationToken cancellationToken)
    {
        return await _reportRepository.GetMemberGrowthAsync(request.Year, cancellationToken);
    }
}
