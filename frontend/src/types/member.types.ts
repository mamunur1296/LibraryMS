import { PagedResult } from "./book.types";

export interface MemberDto {
  id: string;
  firstName: string;
  lastName: string;
  fullName: string;
  email: string;
  phone: string;
  membershipNumber: string;
  address: string | null;
  status: string;
  joinDate: string;
  suspendedUntil: string | null;
  activeBorrows: number;
}

export interface CreateMemberRequest {
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  address?: string;
  username?: string;
  password?: string;
}

export interface UpdateMemberRequest {
  id: string;
  firstName: string;
  lastName: string;
  phone: string;
  address?: string;
}

export interface SuspendMemberRequest {
  id: string;
  suspendedUntil: string;
  reason: string;
}

export type { PagedResult };
