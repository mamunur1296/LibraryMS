using LibraryMS.Application.Contracts.DTOs.Report;
using MediatR;
using System;
using System.Collections.Generic;

namespace LibraryMS.Application.Contracts.Reports;

public class GetLibrarianActivityReportQuery : IRequest<List<LibrarianActivityDto>>
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }

    public GetLibrarianActivityReportQuery(DateTime? fromDate = null, DateTime? toDate = null)
    {
        FromDate = fromDate;
        ToDate = toDate;
    }
}
