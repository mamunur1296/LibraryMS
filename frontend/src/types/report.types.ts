import { PagedResult } from "./book.types";

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

export interface OverdueReportDto {
  borrowId: string;
  memberName: string;
  membershipNumber: string;
  memberEmail: string;
  bookTitle: string;
  branchName: string;
  borrowDate: string;
  dueDate: string;
  overdueDays: number;
  accruedFine: number;
}

export interface PopularBookDto {
  bookId: string;
  title: string;
  authorName: string;
  categoryName: string;
  totalBorrows: number;
}

export interface MemberActivityDto {
  memberId: string;
  fullName: string;
  membershipNumber: string;
  totalBorrows: number;
  activeBorrows: number;
  overdueBorrows: number;
  totalFinesPaid: number;
}

export interface ReportRequest {
  fromDate?: string;
  toDate?: string;
  branchId?: string;
  page?: number;
  pageSize?: number;
}

export interface BranchComparisonDto {
  branchId: string;
  branchName: string;
  totalBooks: number;
  activeBorrows: number;
  overdueBorrows: number;
  totalRevenue: number;
}

export interface AnnualRevenueDto {
  month: number;
  monthName: string;
  revenue: number;
}

export interface MemberGrowthDto {
  month: number;
  monthName: string;
  newMembers: number;
}

export interface LibrarianActivityDto {
  userId: string;
  name: string;
  branchName: string;
  booksIssued: number;
  booksReturned: number;
}

export type { PagedResult };
