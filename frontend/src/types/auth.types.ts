export type Role = "Admin" | "Librarian" | "Member";

export interface User {
  id: string;
  username: string;
  email: string;
  role: string;
  memberId?: string | null;
  branchId?: string | null;
  branchName?: string | null;
  isActive?: boolean;
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string; // ISO date string from backend DateTime
  user: User;        // Backend returns full user object on login
}
