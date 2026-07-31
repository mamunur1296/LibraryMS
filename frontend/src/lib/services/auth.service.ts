import { apiClient } from "@/lib/api-client";
import { AuthResponse } from "@/types/auth.types";
import { LoginFormData, RegisterFormData } from "@/lib/validations/auth.schema";

export const authService = {
  async login(credentials: LoginFormData): Promise<AuthResponse> {
    const response = await apiClient.post<AuthResponse>("/api/Auth/login", credentials);
    return response.data;
  },

  async register(data: RegisterFormData): Promise<string> {
    // Backend API registration endpoint returns status/id
    const response = await apiClient.post<string>("/api/Auth/register", data);
    return response.data;
  },

  async logout(): Promise<void> {
    const refreshToken = localStorage.getItem("refreshToken");
    if (refreshToken) {
      try {
        // Call revoke API on backend to invalidate token session
        await apiClient.post("/api/Auth/revoke", { refreshToken });
      } catch (e) {
        // Fail silently and clear client side storage
        console.error("Failed to revoke session on server:", e);
      }
    }
    localStorage.removeItem("token");
    localStorage.removeItem("refreshToken");
  },

  getToken(): string | null {
    return localStorage.getItem("token");
  },

  getRefreshToken(): string | null {
    return localStorage.getItem("refreshToken");
  },

  isTokenValid(): boolean {
    const token = localStorage.getItem("token");
    if (!token) return false;
    try {
      const payload = JSON.parse(atob(token.split(".")[1]));
      // exp is in seconds, Date.now() is in milliseconds
      return payload.exp * 1000 > Date.now();
    } catch {
      return false;
    }
  },

  getUserRole(): string | null {
    return this.getUserFromToken()?.role ?? null;
  },

  getUserFromToken(): import("../../types/auth.types").User | null {
    const token = localStorage.getItem("token");
    if (!token) return null;
    try {
      const p = JSON.parse(atob(token.split(".")[1]));
      const role =
        p.role ??
        p["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] ??
        null;
      if (!role) return null;
      return {
        id: p.sub ?? p.nameid ?? p["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] ?? "",
        username: p.unique_name ?? p["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] ?? "",
        email: p.email ?? "",
        role,
        memberId: p.memberId ?? null,
        branchId: p.branchId ?? null,
        branchName: p.branchName ?? null,
        isActive: true,
      };
    } catch {
      return null;
    }
  },
};
