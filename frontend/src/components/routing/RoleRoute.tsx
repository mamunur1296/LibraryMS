import { Navigate, Outlet } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import { Role } from '../../types/auth.types';

export const RoleRoute = ({ allowedRoles }: { allowedRoles: Role[] }) => {
  const { user, isAuthenticated } = useAuth();

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  if (user && !allowedRoles.includes(user.role as Role)) {
    return <Navigate to="/unauthorized" replace />;
  }

  return <Outlet />;
};

export const AdminRoute = () => <RoleRoute allowedRoles={['Admin']} />;
export const LibrarianRoute = () => <RoleRoute allowedRoles={['Librarian', 'Admin']} />;
export const MemberRoute = () => <RoleRoute allowedRoles={['Member']} />;
