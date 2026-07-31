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
  hasAccount: boolean;
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

<<<<<<< Updated upstream
=======
<<<<<<< Updated upstream
=======
>>>>>>> Stashed changes
export interface MemberProfileStatsDto {
  memberId: string;
  totalBorrows: number;
  activeBorrows: number;
  overdueBorrows: number;
  activeReservations: number;
  totalFinesDue: number;
<<<<<<< Updated upstream
=======
  totalFinesPaid: number;
  membershipExpiry: string;
  nearestDueDate: string | null;
  favouriteCount: number;
>>>>>>> Stashed changes
}

export interface ResetMemberPasswordRequest {
  newPassword: string;
}

export interface RenewMembershipRequest {
  days: number;
}

export interface CreateMemberUserRequest {
  username: string;
  password?: string;
}

<<<<<<< Updated upstream
=======
>>>>>>> Stashed changes
>>>>>>> Stashed changes
export type { PagedResult };
