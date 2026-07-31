using LibraryMS.Application.Contracts.DTOs.Report;
using MediatR;
using System.Collections.Generic;

namespace LibraryMS.Application.Contracts.Reports;

public class GetBranchComparisonReportQuery : IRequest<List<BranchComparisonDto>>
{
}
