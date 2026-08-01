using FluentAssertions;
using LibraryMS.Application.Contracts.Common;
using LibraryMS.Application.Contracts.DTOs.Report;
using LibraryMS.Application.Contracts.Reports;
using LibraryMS.Application.Contracts.Services;
using LibraryMS.Application.Reports;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace LibraryMS.Application.Tests.Reports;

public class ExportOverdueReportQueryHandlerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IReportExportService> _exportServiceMock;
    private readonly Mock<ILogger<ExportOverdueReportQueryHandler>> _loggerMock;
    
    private readonly ExportOverdueReportQueryHandler _handler;

    public ExportOverdueReportQueryHandlerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _exportServiceMock = new Mock<IReportExportService>();
        _loggerMock = new Mock<ILogger<ExportOverdueReportQueryHandler>>();

        _handler = new ExportOverdueReportQueryHandler(
            _mediatorMock.Object,
            _exportServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ExportPdf_CallsExportServiceWithPdf()
    {
        // Arrange
        var pagedResult = PagedResult<OverdueReportDto>.Create(new List<OverdueReportDto>(), 0, 1, 10000);
        
        _mediatorMock.Setup(x => x.Send(It.IsAny<GetOverdueReportQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        _exportServiceMock.Setup(x => x.ExportToPdfAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new byte[] { 1, 2, 3 });

        var query = new ExportOverdueReportQuery(null, null, null, "pdf");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(new byte[] { 1, 2, 3 });
        
        _exportServiceMock.Verify(x => x.ExportToPdfAsync("Overdue Book Report", It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        _exportServiceMock.Verify(x => x.ExportToExcelAsync(It.IsAny<IEnumerable<OverdueReportDto>>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ExportExcel_CallsExportServiceWithExcel()
    {
        // Arrange
        var pagedResult = PagedResult<OverdueReportDto>.Create(new List<OverdueReportDto>(), 0, 1, 10000);
        
        _mediatorMock.Setup(x => x.Send(It.IsAny<GetOverdueReportQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);

        _exportServiceMock.Setup(x => x.ExportToExcelAsync(It.IsAny<IEnumerable<OverdueReportDto>>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new byte[] { 4, 5, 6 });

        var query = new ExportOverdueReportQuery(null, null, null, "excel");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(new byte[] { 4, 5, 6 });
        
        _exportServiceMock.Verify(x => x.ExportToPdfAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _exportServiceMock.Verify(x => x.ExportToExcelAsync(It.IsAny<IEnumerable<OverdueReportDto>>(), "Overdue Report", It.IsAny<CancellationToken>()), Times.Once);
    }
}
