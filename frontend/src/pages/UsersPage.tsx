import { useEffect, useState } from 'react';
import { userService } from '@/lib/services/user.service';
import { User } from '@/types/auth.types';
import { UserFormModal } from '@/components/users/UserFormModal';
import { useAuth } from '@/contexts/AuthContext';

export default function UsersPage() {
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [userToEdit, setUserToEdit] = useState<User | null>(null);
  const [page, setPage] = useState(1);
  const pageSize = 10;
  const { user: currentUser } = useAuth();

  const fetchUsers = async () => {
    setLoading(true);
    try {
      const data = await userService.getAllUsers();
      // Show only librarians (or admins) in this management page
      setUsers(data.filter(u => u.role !== 'Member'));
      setPage(1);
    } catch (error) {
      console.error('Failed to fetch users', error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchUsers();
  }, []);

  const handleToggleStatus = async (user: User) => {
    if (user.id === currentUser?.id) {
      alert("You cannot suspend yourself.");
      return;
    }
    
    try {
      if (user.isActive) {
        await userService.suspend(user.id);
      } else {
        await userService.activate(user.id);
      }
      fetchUsers();
    } catch (err: unknown) {
      const msg = (err as any)?.response?.data?.message || 'Failed to change status.';
      alert(msg);
    }
  };

  const totalCount = users.length;
  const totalPages = Math.max(1, Math.ceil(totalCount / pageSize));
  const hasPreviousPage = page > 1;
  const hasNextPage = page < totalPages;
  const paginatedUsers = users.slice((page - 1) * pageSize, page * pageSize);

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold tracking-tight text-white">Librarian Management</h1>
          <p className="text-sm text-slate-400 mt-1">Manage library staff and branch assignments.</p>
        </div>
        <button
          onClick={() => {
            setUserToEdit(null);
            setIsModalOpen(true);
          }}
          className="px-4 py-2 bg-indigo-600 hover:bg-indigo-500 text-white rounded-lg shadow-lg shadow-indigo-500/20 text-sm font-medium transition-all flex items-center gap-2"
        >
          <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M18 9v3m0 0v3m0-3h3m-3 0h-3m-2-5a4 4 0 11-8 0 4 4 0 018 0zM3 20a6 6 0 0112 0v1H3v-1z" />
          </svg>
          Add Librarian
        </button>
      </div>

      <div className="bg-slate-900 border border-slate-800 rounded-2xl overflow-hidden shadow-sm flex flex-col">
        <div className="overflow-x-auto">
          <table className="w-full text-left text-sm text-slate-300">
            <thead className="text-xs uppercase bg-slate-900 border-b border-slate-800 text-slate-400">
              <tr>
                <th className="px-6 py-4 font-medium">User</th>
                <th className="px-6 py-4 font-medium">Role</th>
                <th className="px-6 py-4 font-medium">Assigned Branch</th>
                <th className="px-6 py-4 font-medium">Status</th>
                <th className="px-6 py-4 text-right font-medium">Actions</th>
              </tr>
            </thead>
            <tbody>
              {loading ? (
                <tr>
                  <td colSpan={5} className="px-6 py-8 text-center">
                    <div className="flex items-center justify-center">
                      <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-indigo-500"></div>
                    </div>
                  </td>
                </tr>
              ) : users.length === 0 ? (
                <tr>
                  <td colSpan={5} className="px-6 py-8 text-center text-slate-500">
                    No librarians found.
                  </td>
                </tr>
              ) : (
                paginatedUsers.map((user) => (
                  <tr key={user.id} className="border-b border-slate-800/50 hover:bg-slate-800/20 transition-colors">
                    <td className="px-6 py-4">
                      <div className="font-medium text-white">{user.username}</div>
                      <div className="text-xs text-slate-500 mt-0.5">{user.email}</div>
                    </td>
                    <td className="px-6 py-4">
                      <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium border ${
                        user.role === 'Admin' 
                          ? 'bg-fuchsia-500/10 text-fuchsia-400 border-fuchsia-500/20' 
                          : 'bg-indigo-500/10 text-indigo-400 border-indigo-500/20'
                      }`}>
                        {user.role}
                      </span>
                    </td>
                    <td className="px-6 py-4">
                      {user.branchName ? (
                        <div className="text-slate-300">{user.branchName}</div>
                      ) : (
                        <span className="text-slate-500 italic text-xs">Unassigned</span>
                      )}
                    </td>
                    <td className="px-6 py-4">
                      <span
                        className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium border ${
                          user.isActive
                            ? 'bg-emerald-500/10 text-emerald-400 border-emerald-500/20'
                            : 'bg-red-500/10 text-red-400 border-red-500/20'
                        }`}
                      >
                        {user.isActive ? 'Active' : 'Suspended'}
                      </span>
                    </td>
                    <td className="px-6 py-4 text-right space-x-3 whitespace-nowrap">
                      {user.id !== currentUser?.id && (
                        <button
                          onClick={() => handleToggleStatus(user)}
                          className={`text-xs font-medium hover:underline ${
                            user.isActive ? 'text-amber-500' : 'text-emerald-500'
                          }`}
                        >
                          {user.isActive ? 'Suspend' : 'Activate'}
                        </button>
                      )}
                      <button
                        onClick={() => {
                          setUserToEdit(user);
                          setIsModalOpen(true);
                        }}
                        className="text-indigo-400 hover:text-indigo-300 text-xs font-medium"
                      >
                        Assign Branch
                      </button>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>

        {totalCount > 0 && (
          <div className="px-6 py-3 border-t border-slate-800 bg-slate-900/50 flex items-center justify-between">
            <span className="text-xs text-slate-500">
              Showing <span className="font-medium text-slate-300">{(page - 1) * pageSize + 1}</span> to{' '}
              <span className="font-medium text-slate-300">{Math.min(page * pageSize, totalCount)}</span> of{' '}
              <span className="font-medium text-slate-300">{totalCount}</span> users
            </span>
            <div className="flex gap-2">
              <button
                disabled={!hasPreviousPage}
                onClick={() => setPage((p) => p - 1)}
                className="px-3 py-1.5 text-xs font-medium text-slate-300 bg-slate-800 rounded-md hover:bg-slate-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
              >
                Previous
              </button>
              <button
                disabled={!hasNextPage}
                onClick={() => setPage((p) => p + 1)}
                className="px-3 py-1.5 text-xs font-medium text-slate-300 bg-slate-800 rounded-md hover:bg-slate-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
              >
                Next
              </button>
            </div>
          </div>
        )}
      </div>

      <UserFormModal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        user={userToEdit}
        onSuccess={fetchUsers}
      />
    </div>
  );
}
