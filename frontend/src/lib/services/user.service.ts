import { apiClient } from "@/lib/api-client";
import { User } from "@/types/auth.types";

export const userService = {
  async getAllUsers(): Promise<User[]> {
    const response = await apiClient.get<User[]>("/api/Users");
    return response.data;
  },

  async changeRole(targetUserId: string, newRole: string): Promise<void> {
    await apiClient.post("/api/Users/change-role", { targetUserId, newRole });
  },

  async changePassword(data: any): Promise<void> {
    await apiClient.post("/api/Users/change-password", data);
  },

  async changeUsername(data: any): Promise<void> {
    await apiClient.post("/api/Users/change-username", data);
  },

  async changeEmail(data: any): Promise<void> {
    await apiClient.post("/api/Users/change-email", data);
  }
};
