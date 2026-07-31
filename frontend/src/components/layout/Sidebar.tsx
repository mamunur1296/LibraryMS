import * as React from "react";
import { NavLink } from "react-router-dom";
import { useAuth } from "../../contexts/AuthContext";
import { Role } from "../../types/auth.types";
import { BookOpen, Users, Building, FileText, ClipboardList, Settings, UserCircle, LayoutDashboard, Bookmark, Banknote } from "lucide-react";

type NavItem = {
  name: string;
  href: string;
  icon: React.ReactNode;
  roles: Role[];
};

const navigation: NavItem[] = [
  {
    name: "Dashboard",
    href: "/dashboard",
    icon: <LayoutDashboard className="w-5 h-5" />,
    roles: ["Admin", "Librarian", "Member"],
  },
  {
    name: "Books",
    href: "/books",
    icon: <BookOpen className="w-5 h-5" />,
    roles: ["Admin", "Librarian", "Member"],
  },
  {
    name: "Members",
    href: "/members",
    icon: <Users className="w-5 h-5" />,
    roles: ["Admin", "Librarian"],
  },
  {
    name: "Branches",
    href: "/branches",
    icon: <Building className="w-5 h-5" />,
    roles: ["Admin"],
  },
  {
    name: "Librarians",
    href: "/users",
    icon: <UserCircle className="w-5 h-5" />,
    roles: ["Admin"],
  },
  {
    name: "Borrows",
    href: "/borrows",
    icon: <ClipboardList className="w-5 h-5" />,
    roles: ["Admin", "Librarian"],
  },
  {
    name: "My Borrows",
    href: "/my-borrows",
    icon: <ClipboardList className="w-5 h-5" />,
    roles: ["Member"],
  },
  {
    name: "Reservations",
    href: "/reservations",
    icon: <Bookmark className="w-5 h-5" />,
    roles: ["Admin", "Librarian", "Member"],
  },
  {
    name: "Favourites",
    href: "/favourites",
    icon: <Bookmark className="w-5 h-5" />,
    roles: ["Member"],
  },
  {
    name: "My Fines",
    href: "/my-fines",
    icon: <Banknote className="w-5 h-5" />,
    roles: ["Member"],
  },
  {
    name: "Reports",
    href: "/reports",
    icon: <FileText className="w-5 h-5" />,
    roles: ["Admin", "Librarian"],
  },
  {
    name: "Users",
    href: "/users",
    icon: <Users className="w-5 h-5" />,
    roles: ["Admin"],
  },
  {
    name: "Settings",
    href: "/settings",
    icon: <Settings className="w-5 h-5" />,
    roles: ["Admin"],
  },
  {
    name: "Profile",
    href: "/profile",
    icon: <UserCircle className="w-5 h-5" />,
    roles: ["Admin", "Librarian", "Member"],
  },
];

export function Sidebar() {
  const { user } = useAuth();
  
  const filteredNavigation = navigation.filter(item => 
    user && item.roles.includes(user.role as Role)
  );

  return (
    <div className="flex flex-col w-64 bg-slate-900 border-r border-slate-800 h-screen sticky top-0">
      {/* Logo */}
      <div className="flex items-center justify-center h-16 border-b border-slate-800 px-6">
        <h1 className="text-xl font-bold text-white tracking-tight">
          Library<span className="text-indigo-400">MS</span>
        </h1>
      </div>

      {/* Navigation */}
      <div className="flex-1 overflow-y-auto py-4">
        <nav className="px-3 space-y-0.5">
          {filteredNavigation.map((item) => (
            <NavLink
              key={item.name}
              to={item.href}
              className={({ isActive }) =>
                `group flex items-center gap-3 px-3 py-2.5 text-sm font-medium rounded-lg transition-all duration-200 ${
                  isActive
                    ? "bg-indigo-500/10 text-indigo-400 border border-indigo-500/20"
                    : "text-slate-400 hover:bg-slate-800/50 hover:text-slate-200 border border-transparent"
                }`
              }
            >
              <span className="flex-shrink-0">{item.icon}</span>
              <span className="truncate">{item.name}</span>
            </NavLink>
          ))}
        </nav>
      </div>

      {/* Bottom branding */}
      <div className="p-4 border-t border-slate-800">
        <p className="text-xs text-slate-600 text-center">Library Management System</p>
      </div>
    </div>
  );
}
