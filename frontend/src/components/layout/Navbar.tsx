import { useNavigate } from "react-router-dom";
import { useAuth } from "@/contexts/AuthContext";

export function Navbar() {
  const navigate = useNavigate();
  const { user, logout } = useAuth();

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  const initials = user
    ? user.username.slice(0, 2).toUpperCase()
    : "?";

  const roleBadge: Record<string, string> = {
    Admin: "bg-rose-500/20 text-rose-400 border-rose-500/30",
    Librarian: "bg-indigo-500/20 text-indigo-400 border-indigo-500/30",
    Member: "bg-emerald-500/20 text-emerald-400 border-emerald-500/30",
  };

  return (
    <header className="bg-slate-900 border-b border-slate-800 h-16 flex items-center justify-between px-6 sticky top-0 z-10">
      {/* Left: breadcrumb placeholder */}
      <div className="flex items-center">
        <span className="text-sm text-slate-500">Library Management System</span>
      </div>

      {/* Right: user info + logout */}
      <div className="flex items-center gap-4">
        {user && (
          <div className="flex items-center gap-3">
            {/* Role badge */}
            <span
              className={`hidden sm:inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium border ${
                roleBadge[user.role] ?? "bg-slate-700 text-slate-300 border-slate-600"
              }`}
            >
              {user.role}
            </span>

            {/* User avatar + name */}
            <div className="flex items-center gap-2">
              <div className="w-8 h-8 rounded-full bg-gradient-to-tr from-indigo-500 to-purple-500 flex items-center justify-center text-white text-xs font-bold">
                {initials}
              </div>
              <span className="hidden md:block text-sm font-medium text-slate-200">
                {user.username}
              </span>
            </div>
          </div>
        )}

        <button
          onClick={handleLogout}
          className="text-sm font-medium text-slate-300 hover:text-white bg-slate-800 hover:bg-slate-700 px-4 py-2 rounded-lg transition-colors"
        >
          Sign Out
        </button>
      </div>
    </header>
  );
}
