import { useNavigate } from 'react-router-dom';
import { authService } from '@/lib/services/auth.service';

export function Navbar() {
  const navigate = useNavigate();

  const handleLogout = () => {
    authService.logout();
    navigate('/login');
  };

  return (
    <header className="bg-slate-900 border-b border-slate-800 h-16 flex items-center justify-between px-6 sticky top-0 z-10">
      <div className="flex items-center">
        {/* Mobile menu button could go here */}
      </div>

      <div className="flex items-center space-x-4">
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
