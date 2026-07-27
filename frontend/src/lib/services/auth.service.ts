import { apiClient } from "@/lib/api-client";
import { AuthResponse } from "@/types/auth.types";
import { LoginFormData } from "@/lib/validations/auth.schema";

export const authService = {
  async login(credentials: LoginFormData): Promise<AuthResponse> {
    const response = await apiClient.post<AuthResponse>("/api/Auth/login", credentials);
    return response.data;
  },

  logout() {
    if (typeof window !== "undefined") {
      localStorage.removeItem("token");
      localStorage.removeItem("refreshToken");
    }
  },

  getToken() {
    if (typeof window !== "undefined") {
      return localStorage.getItem("token");
    }
    return null;
  }
};
