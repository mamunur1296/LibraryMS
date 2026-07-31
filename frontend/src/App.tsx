import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import { AuthProvider } from "./contexts/AuthContext";
import { DashboardLayout } from "./components/layout/DashboardLayout";
import { PublicLayout } from "./components/layout/PublicLayout";
import { ToastContainer, useToast } from "./components/ui/Toast";
import LoginPage from "./pages/LoginPage";
import RegisterPage from "./pages/RegisterPage";
import DashboardPage from "./pages/DashboardPage";
import BooksPage from "./pages/BooksPage";
import MembersPage from "./pages/MembersPage";
import BorrowsPage from "./pages/BorrowsPage";
import BranchesPage from "./pages/BranchesPage";
import ReservationsPage from "./pages/ReservationsPage";
import ReportsPage from "./pages/ReportsPage";
import SettingsPage from "./pages/SettingsPage";
import UsersPage from "./pages/UsersPage";
import ProfilePage from "./pages/ProfilePage";
import MyFinesPage from "./pages/MyFinesPage";
import CatalogPage from "./pages/CatalogPage";
import FavouritesPage from "./pages/FavouritesPage";
import BookDetailPage from "./pages/BookDetailPage";
import { NotFoundPage } from "./pages/NotFoundPage";
import { ProtectedRoute } from "./components/layout/ProtectedRoute";

function AppInner() {
  const { toasts, removeToast } = useToast();

  return (
    <>
      <ToastContainer toasts={toasts} onRemove={removeToast} />
      <BrowserRouter>
        <Routes>
          {/* Public routes */}
          <Route element={<PublicLayout />}>
            <Route path="/" element={<Navigate to="/catalog" replace />} />
            <Route path="/catalog" element={<CatalogPage />} />
            <Route path="/catalog/:id" element={<BookDetailPage />} />
            <Route path="/favourites" element={<FavouritesPage />} />
            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />
          </Route>

          {/* Protected dashboard routes */}
          <Route
            element={
              <ProtectedRoute>
                <DashboardLayout />
              </ProtectedRoute>
            }
          >
            <Route path="/dashboard" element={<DashboardPage />} />
            <Route path="/books" element={<BooksPage />} />
            <Route path="/members" element={<ProtectedRoute allowedRoles={["Admin", "Librarian"]}><MembersPage /></ProtectedRoute>} />
            <Route path="/borrows" element={<ProtectedRoute allowedRoles={["Admin", "Librarian"]}><BorrowsPage /></ProtectedRoute>} />
            <Route path="/my-borrows" element={<ProtectedRoute allowedRoles={["Member"]}><BorrowsPage /></ProtectedRoute>} />
            <Route path="/branches" element={<ProtectedRoute allowedRoles={["Admin"]}><BranchesPage /></ProtectedRoute>} />
            <Route path="/reservations" element={<ReservationsPage />} />
            <Route path="/my-fines" element={<ProtectedRoute allowedRoles={["Member"]}><MyFinesPage /></ProtectedRoute>} />
            <Route path="/reports" element={<ProtectedRoute allowedRoles={["Admin", "Librarian"]}><ReportsPage /></ProtectedRoute>} />
            <Route path="/settings" element={<ProtectedRoute allowedRoles={["Admin"]}><SettingsPage /></ProtectedRoute>} />
            <Route path="/users" element={<ProtectedRoute allowedRoles={["Admin"]}><UsersPage /></ProtectedRoute>} />
            <Route path="/profile" element={<ProfilePage />} />
          </Route>

          {/* Fallback */}
          <Route path="*" element={<NotFoundPage />} />
        </Routes>
      </BrowserRouter>
    </>
  );
}

function App() {
  return (
    <AuthProvider>
      <AppInner />
    </AuthProvider>
  );
}

export default App;
