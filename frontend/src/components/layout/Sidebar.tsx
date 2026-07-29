import { NavLink } from 'react-router-dom';

const navigation = [
  { name: 'Dashboard', href: '/dashboard' },
  { name: 'Books', href: '/books' },
  { name: 'Members', href: '/members' },
  { name: 'Branches', href: '/branches' },
  { name: 'Borrows', href: '/borrows' },
  { name: 'Reservations', href: '/reservations' },
  { name: 'Reports', href: '/reports' },
  { name: 'Settings', href: '/settings' },
];

export function Sidebar() {
  return (
    <div className="flex flex-col w-64 bg-slate-900 border-r border-slate-800 h-screen sticky top-0">
      <div className="flex items-center justify-center h-16 border-b border-slate-800">
        <h1 className="text-2xl font-bold text-white tracking-tight">
          Library<span className="text-indigo-500">MS</span>
        </h1>
      </div>
      <div className="flex-1 overflow-y-auto py-4">
        <nav className="px-3 space-y-1">
          {navigation.map((item) => (
            <NavLink
              key={item.name}
              to={item.href}
              className={({ isActive }) =>
                `group flex items-center px-3 py-2.5 text-sm font-medium rounded-lg transition-all duration-200 ${
                  isActive
                    ? 'bg-indigo-500/10 text-indigo-400'
                    : 'text-slate-400 hover:bg-slate-800/50 hover:text-slate-200'
                }`
              }
            >
              <span className="truncate">{item.name}</span>
            </NavLink>
          ))}
        </nav>
      </div>
    </div>
  );
}
