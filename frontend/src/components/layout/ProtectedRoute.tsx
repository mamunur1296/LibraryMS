import { Navigate } from "react-router-dom";
import type { ReactNode } from "react";
import { authService } from "@/lib/services/auth.service";

interface ProtectedRouteProps {
  children: ReactNode;
}

export function ProtectedRoute({ children }: ProtectedRouteProps) {
  // Check both token existence AND validity (expiry)
  if (!authService.isTokenValid()) {
    // Clear stale tokens before redirecting
    authService.logout();
    return <Navigate to="/login" replace />;
  }
  return <>{children}</>;
}
