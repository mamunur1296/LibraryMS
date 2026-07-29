import { useState, useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { userService } from '@/lib/services/user.service';
import { apiClient } from '@/lib/api-client';
import { User } from '@/types/auth.types';

const passwordSchema = z.object({
  currentPassword: z.string().min(1, 'Current password is required'),
  newPassword: z.string().min(4, 'New password must be at least 4 characters'),
  confirmPassword: z.string().min(4, 'Confirm password is required'),
}).refine((data) => data.newPassword === data.confirmPassword, {
  message: "Passwords don't match",
  path: ['confirmPassword'],
});

type PasswordFormData = z.infer<typeof passwordSchema>;

export default function SettingsPage() {
  const [activeTab, setActiveTab] = useState<'profile' | 'security' | 'users'>('profile');
  const [currentUser, setCurrentUser] = useState<User | null>(null);
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(true);
  const [username, setUsername] = useState('');
  const [email, setEmail] = useState('');
  const [profileMessage, setProfileMessage] = useState({ type: '', text: '' });

  const { register: registerPwd, handleSubmit: handlePwdSubmit, formState: { errors: pwdErrors, isSubmitting: isPwdSubmitting }, reset: resetPwd } = useForm<PasswordFormData>({
    resolver: zodResolver(passwordSchema),
  });
  const [pwdMessage, setPwdMessage] = useState({ type: '', text: '' });

  useEffect(() => { fetchCurrentUser(); }, []);
  useEffect(() => { if (activeTab === 'users' && currentUser?.role === 'Admin') { fetchUsers(); } }, [activeTab, currentUser]);

  const fetchCurrentUser = async () => {
    try {
      const response = await apiClient.get<User>('/api/Users/me');
      setCurrentUser(response.data);
      setUsername(response.data.username);
      setEmail(response.data.email);
    } catch { console.error('Failed to fetch user'); }
    finally { setLoading(false); }
  };

  const fetchUsers = async () => {
    try { const data = await userService.getAllUsers(); setUsers(data); }
    catch { console.error('Failed to fetch all users'); }
  };

  const onUpdateProfile = async (e: React.FormEvent) => {
    e.preventDefault();
    setProfileMessage({ type: '', text: '' });
    if (!currentUser) return;
    try {
      if (username !== currentUser.username) { await userService.changeUsername({ userId: currentUser.id, newUsername: username }); }
      if (email !== currentUser.email) { await userService.changeEmail({ userId: currentUser.id, newEmail: email }); }
      setProfileMessage({ type: 'success', text: 'Profile updated successfully.' });
      fetchCurrentUser();
    } catch (error: any) {
      setProfileMessage({ type: 'error', text: error.response?.data?.message || 'Failed to update profile.' });
    }
  };

  const onChangePassword = async (data: PasswordFormData) => {
    setPwdMessage({ type: '', text: '' });
    if (!currentUser) return;
    try {
      await userService.changePassword({ userId: currentUser.id, currentPassword: data.currentPassword, newPassword: data.newPassword });
      setPwdMessage({ type: 'success', text: 'Password changed successfully.' });
      resetPwd();
    } catch (error: any) {
      setPwdMessage({ type: 'error', text: error.response?.data?.message || 'Failed to change password.' });
    }
  };

  const onChangeRole = async (userId: string, newRole: string) => {
    try { await userService.changeRole(userId, newRole); fetchUsers(); }
    catch (error: any) { alert(error.response?.data?.message || 'Failed to change role.'); }
  };

  if (loading) return <div className="p-8 text-slate-400">Loading...</div>;

  return (
    <div className="p-8 max-w-6xl mx-auto">
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-white">Settings</h1>
        <p className="text-slate-400 mt-2">Manage your account settings and preferences.</p>
      </div>

      <div className="bg-slate-900 rounded-2xl shadow-sm border border-slate-800 overflow-hidden flex flex-col md:flex-row min-h-[600px]">
        <div className="w-full md:w-64 bg-slate-900/50 border-r border-slate-800 p-6 flex flex-col gap-2">
          {(['profile', 'security'] as const).map((tab) => (
            <button key={tab} onClick={() => setActiveTab(tab)} className={`text-left px-4 py-3 rounded-xl font-medium transition-colors ${activeTab === tab ? 'bg-indigo-500/10 text-indigo-400' : 'text-slate-400 hover:bg-slate-800/50'}`}>
              {tab === 'profile' ? 'Profile Details' : 'Security'}
            </button>
          ))}
          {currentUser?.role === 'Admin' && (
            <button onClick={() => setActiveTab('users')} className={`text-left px-4 py-3 rounded-xl font-medium transition-colors ${activeTab === 'users' ? 'bg-indigo-500/10 text-indigo-400' : 'text-slate-400 hover:bg-slate-800/50'}`}>User Management</button>
          )}
        </div>

        <div className="flex-1 p-8">
          {activeTab === 'profile' && (
            <div className="max-w-xl">
              <h2 className="text-xl font-bold text-white mb-6">Profile Details</h2>
              {profileMessage.text && (
                <div className={`p-4 rounded-xl mb-6 ${profileMessage.type === 'error' ? 'bg-red-500/10 text-red-400 border border-red-500/20' : 'bg-emerald-500/10 text-emerald-400 border border-emerald-500/20'}`}>{profileMessage.text}</div>
              )}
              <form onSubmit={onUpdateProfile} className="space-y-6">
                <div>
                  <label className="block text-sm font-medium text-slate-300 mb-2">Username</label>
                  <input type="text" value={username} onChange={(e) => setUsername(e.target.value)} className="w-full px-4 py-3 rounded-xl border border-slate-700 bg-slate-950 text-white focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 outline-none transition-all" />
                </div>
                <div>
                  <label className="block text-sm font-medium text-slate-300 mb-2">Email Address</label>
                  <input type="email" value={email} onChange={(e) => setEmail(e.target.value)} className="w-full px-4 py-3 rounded-xl border border-slate-700 bg-slate-950 text-white focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 outline-none transition-all" />
                </div>
                <button type="submit" className="px-6 py-3 bg-indigo-600 hover:bg-indigo-700 text-white font-medium rounded-xl transition-colors shadow-sm">Save Changes</button>
              </form>
            </div>
          )}

          {activeTab === 'security' && (
            <div className="max-w-xl">
              <h2 className="text-xl font-bold text-white mb-6">Change Password</h2>
              {pwdMessage.text && (
                <div className={`p-4 rounded-xl mb-6 ${pwdMessage.type === 'error' ? 'bg-red-500/10 text-red-400 border border-red-500/20' : 'bg-emerald-500/10 text-emerald-400 border border-emerald-500/20'}`}>{pwdMessage.text}</div>
              )}
              <form onSubmit={handlePwdSubmit(onChangePassword)} className="space-y-6">
                {(['currentPassword', 'newPassword', 'confirmPassword'] as const).map((field) => (
                  <div key={field}>
                    <label className="block text-sm font-medium text-slate-300 mb-2">{field === 'currentPassword' ? 'Current Password' : field === 'newPassword' ? 'New Password' : 'Confirm New Password'}</label>
                    <input type="password" {...registerPwd(field)} className="w-full px-4 py-3 rounded-xl border border-slate-700 bg-slate-950 text-white focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 outline-none transition-all" />
                    {pwdErrors[field] && <p className="text-red-400 text-sm mt-1">{pwdErrors[field]?.message}</p>}
                  </div>
                ))}
                <button type="submit" disabled={isPwdSubmitting} className="px-6 py-3 bg-slate-800 hover:bg-slate-700 text-white font-medium rounded-xl transition-colors shadow-sm disabled:opacity-70">
                  {isPwdSubmitting ? 'Updating...' : 'Update Password'}
                </button>
              </form>
            </div>
          )}

          {activeTab === 'users' && currentUser?.role === 'Admin' && (
            <div>
              <h2 className="text-xl font-bold text-white mb-6">User Management</h2>
              <div className="overflow-x-auto rounded-xl border border-slate-700">
                <table className="w-full text-left border-collapse">
                  <thead>
                    <tr className="bg-slate-800 border-b border-slate-700">
                      <th className="py-4 px-6 font-medium text-slate-400 text-sm uppercase">Username</th>
                      <th className="py-4 px-6 font-medium text-slate-400 text-sm uppercase">Email</th>
                      <th className="py-4 px-6 font-medium text-slate-400 text-sm uppercase">Role</th>
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-slate-800">
                    {users.map((u) => (
                      <tr key={u.id} className="hover:bg-slate-800/30 transition-colors">
                        <td className="py-4 px-6 text-white font-medium">{u.username}</td>
                        <td className="py-4 px-6 text-slate-400">{u.email}</td>
                        <td className="py-4 px-6">
                          <select value={u.role} onChange={(e) => onChangeRole(u.id, e.target.value)} disabled={u.id === currentUser.id} className="bg-slate-800 border-none rounded-lg px-3 py-1.5 text-sm font-medium text-slate-300 outline-none focus:ring-2 focus:ring-indigo-500 disabled:opacity-50">
                            <option value="Admin">Admin</option>
                            <option value="Librarian">Librarian</option>
                            <option value="Member">Member</option>
                          </select>
                        </td>
                      </tr>
                    ))}
                    {users.length === 0 && (<tr><td colSpan={3} className="py-8 text-center text-slate-500">No users found</td></tr>)}
                  </tbody>
                </table>
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
