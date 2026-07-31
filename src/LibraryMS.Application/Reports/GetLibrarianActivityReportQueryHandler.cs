using LibraryMS.Application.Contracts.DTOs.Report;
using LibraryMS.Application.Contracts.Reports;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryMS.Application.Reports;

public class GetLibrarianActivityReportQueryHandler : IRequestHandler<GetLibrarianActivityReportQuery, List<LibrarianActivityDto>>
{
    private readonly IReportRepository _reportRepository;

    public GetLibrarianActivityReportQueryHandler(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task<List<LibrarianActivityDto>> Handle(GetLibrarianActivityReportQuery request, CancellationToken cancellationToken)
    {
        return await _reportRepository.GetLibrarianActivityAsync(request.FromDate, request.ToDate, cancellationToken);
    }
}
