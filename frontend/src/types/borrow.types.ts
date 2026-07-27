import { PagedResult } from "./book.types";

export interface BorrowDto {
  id: string;
  memberId: string;
  memberName: string;
  membershipNumber: string;
  bookId: string;
  bookTitle: string;
  bookISBN: string;
  copyNumber: string;
  branchId: string;
  branchName: string;
  borrowDate: string;
  dueDate: string;
  returnDate: string | null;
  status: string;
  lateFine: number;
  isFinePaid: boolean;
  isOverdue: boolean;
  daysUntilDue: number;
}

export interface BorrowBookRequest {
  memberId: string;
  bookId: string;
  bookCopyId: string;
  branchId: string;
  borrowDays?: number;
}

export interface ReturnBookRequest {
  borrowId: string;
  notes?: string;
}

export type { PagedResult };
