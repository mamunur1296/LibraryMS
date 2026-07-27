import { PagedResult } from "./book.types";

export interface ReservationDto {
  id: string;
  memberId: string;
  memberName: string;
  membershipNumber: string;
  bookId: string;
  bookTitle: string;
  bookISBN: string;
  branchId: string;
  branchName: string;
  queuePosition: number;
  status: string;
  createdAt: string;
  notifiedAt: string | null;
  expiresAt: string | null;
}

export interface CreateReservationRequest {
  memberId: string;
  bookId: string;
  branchId: string;
}

export interface ReservationQueueDto {
  bookId: string;
  bookTitle: string;
  branchId: string;
  branchName: string;
  totalInQueue: number;
  queue: ReservationDto[];
}

export type { PagedResult };
