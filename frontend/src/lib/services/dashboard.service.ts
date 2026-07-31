import { apiClient } from "@/lib/api-client";
import { AdminDashboardSummaryDto, DashboardSummaryDto } from "@/types/dashboard.types";

export const dashboardService = {
  async getSummary(): Promise<DashboardSummaryDto> {
    const response = await apiClient.get<DashboardSummaryDto>("/api/Reports/dashboard-summary");
    return response.data;
  },

  async getAdminSummary(): Promise<AdminDashboardSummaryDto> {
    const response = await apiClient.get<AdminDashboardSummaryDto>("/api/Reports/admin-dashboard-summary");
    return response.data;
  },
};
