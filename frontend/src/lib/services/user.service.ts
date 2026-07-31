import { apiClient } from "@/lib/api-client";
import { User } from "@/types/auth.types";

interface ChangeRoleRequest {
  targetUserId: string;
  newRole: string;
}

interface ChangePasswordRequest {
  userId: string;
  currentPassword: string;
  newPassword: string;
}

interface ChangeUsernameRequest {
  userId: string;
  newUsername: string;
}

interface ChangeEmailRequest {
  userId: string;
  newEmail: string;
}

export interface CreateLibrarianRequest {
  username: string;
  email: string;
  password?: string;
  branchId?: string;
}

export const userService = {
  async getAllUsers(): Promise<User[]> {
    const response = await apiClient.get<User[]>("/api/Users");
    return response.data;
  },

  async changeRole(targetUserId: string, newRole: string): Promise<void> {
    const body: ChangeRoleRequest = { targetUserId, newRole };
    await apiClient.post("/api/Users/change-role", body);
  },

  async changePassword(data: ChangePasswordRequest): Promise<void> {
    await apiClient.post("/api/Users/change-password", data);
  },

  async changeUsername(data: ChangeUsernameRequest): Promise<void> {
    await apiClient.post("/api/Users/change-username", data);
  },

  async changeEmail(data: ChangeEmailRequest): Promise<void> {
    await apiClient.post("/api/Users/change-email", data);
  },

  async createLibrarian(data: CreateLibrarianRequest): Promise<string> {
    const response = await apiClient.post<string>("/api/Users/create-librarian", data);
    return response.data;
  },

  async assignBranch(userId: string, branchId: string): Promise<void> {
    await apiClient.post(`/api/Users/${userId}/assign-branch`, { branchId });
  },

  async suspend(userId: string): Promise<void> {
    await apiClient.post(`/api/Users/${userId}/suspend`);
  },

  async activate(userId: string): Promise<void> {
    await apiClient.post(`/api/Users/${userId}/activate`);
  }
};
