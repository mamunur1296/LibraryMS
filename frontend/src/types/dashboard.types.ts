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
