export interface DashboardSummaryDto {
  totalBooks: number;
  totalMembers: number;
  activeBorrows: number;
  overdueBorrows: number;
  pendingReservations: number;
  totalBranches: number;
  totalLateFinesCollected: number;
  pendingLateFines: number;
}

export interface BranchDashboardSummaryDto {
  branchId: string;
  branchName: string;
  totalBooks: number;
  totalMembers: number;
  activeBorrows: number;
  overdueBorrows: number;
  totalRevenue: number;
}

export interface AdminDashboardSummaryDto {
  totalSummary: DashboardSummaryDto;
  branchSummaries: BranchDashboardSummaryDto[];
}
