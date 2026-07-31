import { useState, useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { userService } from '@/lib/services/user.service';
import { settingsService } from '@/lib/services/settings.service';
import { useAuth } from '@/contexts/AuthContext';
import { toast } from '@/components/ui/Toast';
import type { User } from '@/types/auth.types';

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
  const [activeTab, setActiveTab] = useState<'profile' | 'security' | 'system'>('profile');
  const { user: authUser, isLoading: authLoading, refreshUser } = useAuth();
  const [currentUser, setCurrentUser] = useState<User | null>(null);
  
  // System Settings state
  const [systemSettings, setSystemSettings] = useState<{key: string, value: string}[]>([]);
  const [finePerDay, setFinePerDay] = useState('2.00');
  const [maxBorrowDays, setMaxBorrowDays] = useState('14');
  const [globalBorrowLimit, setGlobalBorrowLimit] = useState('5');
  const [reservationExpiryDays, setReservationExpiryDays] = useState('3');
  const [emailNotificationsEnabled, setEmailNotificationsEnabled] = useState(true);
  const [bookCoverUploadEnabled, setBookCoverUploadEnabled] = useState(false);
  const [onlinePaymentEnabled, setOnlinePaymentEnabled] = useState(false);
  const [emailVerificationEnabled, setEmailVerificationEnabled] = useState(false);
  const [defaultTheme, setDefaultTheme] = useState('Dark');
  const [barcodeScanEnabled, setBarcodeScanEnabled] = useState(false);
  
  const [savingSettings, setSavingSettings] = useState(false);
  const [loading, setLoading] = useState(true);
  const [username, setUsername] = useState('');
  const [email, setEmail] = useState('');
  const { register: registerPwd, handleSubmit: handlePwdSubmit, formState: { errors: pwdErrors, isSubmitting: isPwdSubmitting }, reset: resetPwd } = useForm<PasswordFormData>({
    resolver: zodResolver(passwordSchema),
  });

  useEffect(() => {
    if (!authLoading) {
      if (authUser) {
        setCurrentUser(authUser);
        setUsername(authUser.username);
        setEmail(authUser.email);
      }
      setLoading(false);
    }
  }, [authUser, authLoading]);
  
  useEffect(() => {
    if (activeTab === 'system' && currentUser?.role === 'Admin') {
      void fetchSystemSettings();
    }
  }, [activeTab, currentUser]);

  const fetchSystemSettings = async () => {
    try {
      const data = await settingsService.getSettings();
      setSystemSettings(data);
      
      const getValue = (key: string) => data.find(s => s.key === key)?.value;
      
      if (getValue('FinePerDay')) setFinePerDay(getValue('FinePerDay')!);
      if (getValue('MaxBorrowDays')) setMaxBorrowDays(getValue('MaxBorrowDays')!);
      if (getValue('GlobalBorrowLimit')) setGlobalBorrowLimit(getValue('GlobalBorrowLimit')!);
      if (getValue('ReservationExpiryDays')) setReservationExpiryDays(getValue('ReservationExpiryDays')!);
      if (getValue('EmailNotificationsEnabled')) setEmailNotificationsEnabled(getValue('EmailNotificationsEnabled') === 'true');
      if (getValue('BookCoverUploadEnabled')) setBookCoverUploadEnabled(getValue('BookCoverUploadEnabled') === 'true');
      if (getValue('OnlinePaymentEnabled')) setOnlinePaymentEnabled(getValue('OnlinePaymentEnabled') === 'true');
      if (getValue('EmailVerificationEnabled')) setEmailVerificationEnabled(getValue('EmailVerificationEnabled') === 'true');
      if (getValue('DefaultTheme')) setDefaultTheme(getValue('DefaultTheme')!);
      if (getValue('BarcodeScanEnabled')) setBarcodeScanEnabled(getValue('BarcodeScanEnabled') === 'true');
    } catch {
      toast.error('Failed to load system settings');
    }
  };

  const handleSaveSystemSettings = async (e: React.FormEvent) => {
    e.preventDefault();
    setSavingSettings(true);
    try {
      await settingsService.updateSetting('FinePerDay', { value: finePerDay });
      await settingsService.updateSetting('MaxBorrowDays', { value: maxBorrowDays });
      await settingsService.updateSetting('GlobalBorrowLimit', { value: globalBorrowLimit });
      await settingsService.updateSetting('ReservationExpiryDays', { value: reservationExpiryDays });
      await settingsService.updateSetting('EmailNotificationsEnabled', { value: emailNotificationsEnabled ? 'true' : 'false' });
      await settingsService.updateSetting('BookCoverUploadEnabled', { value: bookCoverUploadEnabled ? 'true' : 'false' });
      await settingsService.updateSetting('OnlinePaymentEnabled', { value: onlinePaymentEnabled ? 'true' : 'false' });
      await settingsService.updateSetting('EmailVerificationEnabled', { value: emailVerificationEnabled ? 'true' : 'false' });
      await settingsService.updateSetting('DefaultTheme', { value: defaultTheme });
      await settingsService.updateSetting('BarcodeScanEnabled', { value: barcodeScanEnabled ? 'true' : 'false' });
      
      toast.success('System settings updated successfully');
      await fetchSystemSettings();
    } catch {
      toast.error('Failed to update system settings');
    } finally {
      setSavingSettings(false);
    }
  };

  const onUpdateProfile = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!currentUser) return;
    try {
      if (username !== currentUser.username) {
        await userService.changeUsername({ userId: currentUser.id, newUsername: username });
      }
      if (email !== currentUser.email) {
        await userService.changeEmail({ userId: currentUser.id, newEmail: email });
      }
      toast.success('Profile updated successfully.');
      refreshUser();
    } catch {
      toast.error('Failed to update profile');
    }
  };

  const onChangePassword = async (data: PasswordFormData) => {
    if (!currentUser) return;
    try {
      await userService.changePassword({
        userId: currentUser.id,
        currentPassword: data.currentPassword,
        newPassword: data.newPassword,
      });
      toast.success('Password changed successfully');
      resetPwd();
    } catch {
      toast.error('Failed to change password. Please check your current password.');
    }
  };

  if (loading) return null;

  return (
    <div className="max-w-4xl space-y-8">
      <div>
        <h1 className="text-2xl font-bold tracking-tight text-white">Settings</h1>
        <p className="text-sm text-slate-400 mt-1">Manage your profile, security, and system preferences.</p>
      </div>

      <div className="flex flex-col md:flex-row gap-8 bg-slate-900 border border-slate-800 rounded-2xl overflow-hidden shadow-sm">
        <div className="md:w-64 flex flex-col gap-1 p-4 border-r border-slate-800 bg-slate-900/50">
          <button
            onClick={() => setActiveTab('profile')}
            className={`flex items-center gap-3 px-4 py-3 rounded-xl text-sm font-medium transition-all ${
              activeTab === 'profile'
                ? 'bg-indigo-500/10 text-indigo-400 border border-indigo-500/20 shadow-sm'
                : 'text-slate-400 hover:text-slate-300 hover:bg-slate-800'
            }`}
          >
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" /></svg>
            Profile
          </button>

          <button
            onClick={() => setActiveTab('security')}
            className={`flex items-center gap-3 px-4 py-3 rounded-xl text-sm font-medium transition-all ${
              activeTab === 'security'
                ? 'bg-indigo-500/10 text-indigo-400 border border-indigo-500/20 shadow-sm'
                : 'text-slate-400 hover:text-slate-300 hover:bg-slate-800'
            }`}
          >
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z" /></svg>
            Security
          </button>

          {currentUser?.role === 'Admin' && (
            <button
              onClick={() => setActiveTab('system')}
              className={`flex items-center gap-3 px-4 py-3 rounded-xl text-sm font-medium transition-all ${
                activeTab === 'system'
                  ? 'bg-indigo-500/10 text-indigo-400 border border-indigo-500/20 shadow-sm'
                  : 'text-slate-400 hover:text-slate-300 hover:bg-slate-800'
              }`}
            >
              <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z" /><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" /></svg>
              System Settings
            </button>
          )}
        </div>

        <div className="flex-1 p-8">
          {activeTab === 'profile' && (
            <div className="max-w-xl">
              <h2 className="text-xl font-bold text-white mb-6">Profile Details</h2>
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

          {activeTab === 'system' && currentUser?.role === 'Admin' && (
            <div>
              <h2 className="text-xl font-bold text-white mb-6">System Configuration</h2>
              <form onSubmit={handleSaveSystemSettings} className="space-y-6 max-w-lg">
                <div>
                  <label className="block text-sm font-medium text-slate-300 mb-2">Fine Per Day ($)</label>
                  <input type="number" step="0.01" min="0" value={finePerDay} onChange={(e) => setFinePerDay(e.target.value)} className="w-full bg-slate-900 border border-slate-700 rounded-xl px-4 py-3 text-white focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 transition-colors" />
                </div>
                
                <div>
                  <label className="block text-sm font-medium text-slate-300 mb-2">Maximum Borrow Days</label>
                  <input type="number" min="1" value={maxBorrowDays} onChange={(e) => setMaxBorrowDays(e.target.value)} className="w-full bg-slate-900 border border-slate-700 rounded-xl px-4 py-3 text-white focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 transition-colors" />
                </div>
                
                <div>
                  <label className="block text-sm font-medium text-slate-300 mb-2">Global Borrow Limit Per Member</label>
                  <input type="number" min="1" value={globalBorrowLimit} onChange={(e) => setGlobalBorrowLimit(e.target.value)} className="w-full bg-slate-900 border border-slate-700 rounded-xl px-4 py-3 text-white focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 transition-colors" />
                </div>

                <div>
                  <label className="block text-sm font-medium text-slate-300 mb-2">Reservation Expiry Days</label>
                  <input type="number" min="1" value={reservationExpiryDays} onChange={(e) => setReservationExpiryDays(e.target.value)} className="w-full bg-slate-900 border border-slate-700 rounded-xl px-4 py-3 text-white focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 transition-colors" />
                </div>

                <div className="flex items-center justify-between p-4 bg-slate-950 rounded-xl border border-slate-800">
                  <div>
                    <div className="font-medium text-slate-200">Email Notifications</div>
                    <div className="text-xs text-slate-500">Enable system-wide email alerts</div>
                  </div>
                  <label className="relative inline-flex items-center cursor-pointer">
                    <input type="checkbox" checked={emailNotificationsEnabled} onChange={(e) => setEmailNotificationsEnabled(e.target.checked)} className="sr-only peer" />
                    <div className="w-11 h-6 bg-slate-700 peer-focus:outline-none rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-indigo-500"></div>
                  </label>
                </div>

                <div className="flex items-center justify-between p-4 bg-slate-950 rounded-xl border border-slate-800">
                  <div>
                    <div className="font-medium text-slate-200">Book Cover Uploads</div>
                    <div className="text-xs text-slate-500">Allow librarians to upload cover images</div>
                  </div>
                  <label className="relative inline-flex items-center cursor-pointer">
                    <input type="checkbox" checked={bookCoverUploadEnabled} onChange={(e) => setBookCoverUploadEnabled(e.target.checked)} className="sr-only peer" />
                    <div className="w-11 h-6 bg-slate-700 peer-focus:outline-none rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-indigo-500"></div>
                  </label>
                </div>

                <div className="flex items-center justify-between p-4 bg-slate-950 rounded-xl border border-slate-800">
                  <div>
                    <div className="font-medium text-slate-200">Online Payments</div>
                    <div className="text-xs text-slate-500">Enable online fine payments</div>
                  </div>
                  <label className="relative inline-flex items-center cursor-pointer">
                    <input type="checkbox" checked={onlinePaymentEnabled} onChange={(e) => setOnlinePaymentEnabled(e.target.checked)} className="sr-only peer" />
                    <div className="w-11 h-6 bg-slate-700 peer-focus:outline-none rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-indigo-500"></div>
                  </label>
                </div>
                
                <div className="flex items-center justify-between p-4 bg-slate-950 rounded-xl border border-slate-800">
                  <div>
                    <div className="font-medium text-slate-200">Email Verification</div>
                    <div className="text-xs text-slate-500">Require email verification for new members</div>
                  </div>
                  <label className="relative inline-flex items-center cursor-pointer">
                    <input type="checkbox" checked={emailVerificationEnabled} onChange={(e) => setEmailVerificationEnabled(e.target.checked)} className="sr-only peer" />
                    <div className="w-11 h-6 bg-slate-700 peer-focus:outline-none rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-indigo-500"></div>
                  </label>
                </div>

                <div className="flex items-center justify-between p-4 bg-slate-950 rounded-xl border border-slate-800">
                  <div>
                    <div className="font-medium text-slate-200">QR/Barcode Scanning</div>
                    <div className="text-xs text-slate-500">Enable barcode scanner integration</div>
                  </div>
                  <label className="relative inline-flex items-center cursor-pointer">
                    <input type="checkbox" checked={barcodeScanEnabled} onChange={(e) => setBarcodeScanEnabled(e.target.checked)} className="sr-only peer" />
                    <div className="w-11 h-6 bg-slate-700 peer-focus:outline-none rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-indigo-500"></div>
                  </label>
                </div>

                <div>
                  <label className="block text-sm font-medium text-slate-300 mb-2">Default Theme</label>
                  <select value={defaultTheme} onChange={(e) => setDefaultTheme(e.target.value)} className="w-full bg-slate-900 border border-slate-700 rounded-xl px-4 py-3 text-white focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 transition-colors">
                    <option value="Dark">Dark Mode</option>
                    <option value="Light">Light Mode</option>
                    <option value="System">System Default</option>
                  </select>
                </div>
                
                <button
                  type="submit"
                  disabled={savingSettings}
                  className="px-6 py-3 bg-indigo-600 hover:bg-indigo-500 text-white rounded-xl font-medium transition-all shadow-lg shadow-indigo-500/20 disabled:opacity-50 disabled:cursor-not-allowed w-full"
                >
                  {savingSettings ? 'Saving Configuration...' : 'Save Configuration'}
                </button>
              </form>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
