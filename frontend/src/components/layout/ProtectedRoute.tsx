import { Navigate } from "react-router-dom";
import type { ReactNode } from "react";
import { authService } from "@/lib/services/auth.service";
import { useAuth } from "@/contexts/AuthContext";
import { Role } from "@/types/auth.types";

interface ProtectedRouteProps {
  children: ReactNode;
  allowedRoles?: Role[];
}

export function ProtectedRoute({ children, allowedRoles }: ProtectedRouteProps) {
  const { user } = useAuth();

  // Check both token existence AND validity (expiry)
  if (!authService.isTokenValid()) {
    // Clear stale tokens before redirecting
    authService.logout();
    return <Navigate to="/login" replace />;
  }

  if (allowedRoles && user && !allowedRoles.includes(user.role as Role)) {
    return <Navigate to="/dashboard" replace />;
  }

  return <>{children}</>;
}
