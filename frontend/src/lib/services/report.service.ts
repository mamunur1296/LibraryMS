import { apiClient } from "../api-client";
import { 
  DashboardSummaryDto, 
  OverdueReportDto, 
  PopularBookDto, 
  MemberActivityDto, 
  ReportRequest, 
  PagedResult 
} from "../../types/report.types";

export const reportService = {
  async getDashboardSummary(): Promise<DashboardSummaryDto> {
    const response = await apiClient.get<DashboardSummaryDto>("/api/Reports/dashboard");
    return response.data;
  },

  async getOverdueReport(params: ReportRequest): Promise<PagedResult<OverdueReportDto>> {
    const queryParams = new URLSearchParams();
    if (params.fromDate) queryParams.append("fromDate", params.fromDate);
    if (params.toDate) queryParams.append("toDate", params.toDate);
    if (params.branchId) queryParams.append("branchId", params.branchId);
    if (params.page) queryParams.append("page", params.page.toString());
    if (params.pageSize) queryParams.append("pageSize", params.pageSize.toString());

    const response = await apiClient.get<PagedResult<OverdueReportDto>>(`/api/Reports/overdue?${queryParams.toString()}`);
    return response.data;
  },

  async getPopularBooks(params: ReportRequest): Promise<PagedResult<PopularBookDto>> {
    const queryParams = new URLSearchParams();
    if (params.fromDate) queryParams.append("fromDate", params.fromDate);
    if (params.toDate) queryParams.append("toDate", params.toDate);
    if (params.branchId) queryParams.append("branchId", params.branchId);
    if (params.page) queryParams.append("page", params.page.toString());
    if (params.pageSize) queryParams.append("pageSize", params.pageSize.toString());

    const response = await apiClient.get<PagedResult<PopularBookDto>>(`/api/Reports/popular-books?${queryParams.toString()}`);
    return response.data;
  },

  async getMemberActivity(params: ReportRequest): Promise<PagedResult<MemberActivityDto>> {
    const queryParams = new URLSearchParams();
    if (params.fromDate) queryParams.append("fromDate", params.fromDate);
    if (params.toDate) queryParams.append("toDate", params.toDate);
    if (params.page) queryParams.append("page", params.page.toString());
    if (params.pageSize) queryParams.append("pageSize", params.pageSize.toString());

    const response = await apiClient.get<PagedResult<MemberActivityDto>>(`/api/Reports/member-activity?${queryParams.toString()}`);
    return response.data;
  },

  async exportOverdueReport(params: ReportRequest, format: "excel" | "pdf"): Promise<void> {
    const queryParams = new URLSearchParams();
    if (params.fromDate) queryParams.append("fromDate", params.fromDate);
    if (params.toDate) queryParams.append("toDate", params.toDate);
    if (params.branchId) queryParams.append("branchId", params.branchId);
    queryParams.append("format", format);

    const response = await apiClient.get(`/api/Reports/overdue/export?${queryParams.toString()}`, {
      responseType: "blob",
    });

    // Handle file download
    const url = window.URL.createObjectURL(new Blob([response.data]));
    const link = document.createElement("a");
    link.href = url;
    
    // Attempt to extract filename from content-disposition header if available, otherwise use default
    const contentDisposition = response.headers["content-disposition"];
    let fileName = `OverdueReport_${new Date().toISOString().split('T')[0]}.${format === "excel" ? "xlsx" : "pdf"}`;
    if (contentDisposition) {
      const fileNameMatch = contentDisposition.match(/filename="?([^"]+)"?/);
      if (fileNameMatch && fileNameMatch.length === 2) {
        fileName = fileNameMatch[1];
      }
    }
    
    link.setAttribute("download", fileName);
    document.body.appendChild(link);
    link.click();
    link.remove();
    window.URL.revokeObjectURL(url);
  }
};
