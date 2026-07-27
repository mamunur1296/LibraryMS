import { apiClient } from "@/lib/api-client";
import { DashboardSummaryDto } from "@/types/dashboard.types";

export const dashboardService = {
  async getSummary(): Promise<DashboardSummaryDto> {
    const response = await apiClient.get<DashboardSummaryDto>("/api/Reports/dashboard");
    return response.data;
  },
};
