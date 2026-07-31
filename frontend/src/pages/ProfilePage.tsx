import { useState, useEffect, useCallback } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { memberService } from '@/lib/services/member.service';
import { borrowService } from '@/lib/services/borrow.service';
import { userService } from '@/lib/services/user.service';
import { useAuth } from '@/contexts/AuthContext';
import { toast } from '@/components/ui/Toast';
import type { MemberDto } from '@/types/member.types';
import type { BorrowDto, PagedResult } from '@/types/borrow.types';
import { UserCircle, Shield, CreditCard, History, Image as ImageIcon } from 'lucide-react';

const personalInfoSchema = z.object({
  firstName: z.string().min(2, 'First name is required'),
  lastName: z.string().min(2, 'Last name is required'),
  phone: z.string().min(10, 'Valid phone number is required'),
  address: z.string().optional(),
});
type PersonalInfoData = z.infer<typeof personalInfoSchema>;

const passwordSchema = z.object({
  currentPassword: z.string().min(1, 'Current password is required'),
  newPassword: z.string().min(4, 'New password must be at least 4 characters'),
  confirmPassword: z.string().min(4, 'Confirm password is required'),
}).refine((data) => data.newPassword === data.confirmPassword, {
  message: "Passwords don't match",
  path: ['confirmPassword'],
});
type PasswordData = z.infer<typeof passwordSchema>;

export default function ProfilePage() {
  const { user } = useAuth();
  const [activeTab, setActiveTab] = useState<'personal' | 'security' | 'membership' | 'history' | 'photo'>('personal');
  const [memberInfo, setMemberInfo] = useState<MemberDto | null>(null);
  const [loading, setLoading] = useState(true);
  
  // Borrow history state
  const [borrows, setBorrows] = useState<PagedResult<BorrowDto> | null>(null);
  const [borrowPage, setBorrowPage] = useState(1);
  const [borrowStatus, setBorrowStatus] = useState("");

  const { register: regPersonal, handleSubmit: handlePersonalSubmit, reset: resetPersonal, formState: { isSubmitting: isPersonalSubmitting } } = useForm<PersonalInfoData>({
    resolver: zodResolver(personalInfoSchema)
  });

  const { register: regPassword, handleSubmit: handlePasswordSubmit, reset: resetPassword, formState: { errors: pwdErrors, isSubmitting: isPwdSubmitting } } = useForm<PasswordData>({
    resolver: zodResolver(passwordSchema)
  });

  const loadProfile = useCallback(async () => {
    if (!user?.memberId) return;
    try {
      setLoading(true);
      const data = await memberService.getById(user.memberId);
      setMemberInfo(data);
      resetPersonal({
        firstName: data.firstName,
        lastName: data.lastName,
        phone: data.phone,
        address: data.address || '',
      });
    } catch {
      toast.error("Failed to load profile data");
    } finally {
      setLoading(false);
    }
  }, [user, resetPersonal]);

  useEffect(() => {
    void loadProfile();
  }, [loadProfile]);

  const fetchBorrows = useCallback(async () => {
    if (!user?.memberId) return;
    try {
      const data = await borrowService.search(user.memberId, undefined, borrowStatus || undefined, borrowPage, 10);
      setBorrows(data);
    } catch {
      toast.error("Failed to load borrow history");
    }
  }, [user, borrowStatus, borrowPage]);

  useEffect(() => {
    if (activeTab === 'history') {
      void fetchBorrows();
    }
  }, [activeTab, fetchBorrows]);

  const onPersonalSubmit = async (data: PersonalInfoData) => {
    if (!user?.memberId) return;
    try {
      await memberService.update(user.memberId, data as any);
      toast.success("Personal information updated");
      void loadProfile();
    } catch {
      toast.error("Failed to update personal information");
    }
  };

  const onPasswordSubmit = async (data: PasswordData) => {
    if (!user?.id) return;
    try {
      await userService.changePassword({
        userId: user.id,
        currentPassword: data.currentPassword,
        newPassword: data.newPassword
      });
      toast.success("Password changed successfully");
      resetPassword();
    } catch {
      toast.error("Failed to change password. Check your current password.");
    }
  };

  const tabs = [
    { id: 'personal', label: 'Personal Info', icon: <UserCircle className="w-4 h-4 mr-2" /> },
    { id: 'security', label: 'Security', icon: <Shield className="w-4 h-4 mr-2" /> },
    { id: 'membership', label: 'Membership', icon: <CreditCard className="w-4 h-4 mr-2" /> },
    { id: 'history', label: 'Borrow History', icon: <History className="w-4 h-4 mr-2" /> },
    { id: 'photo', label: 'Profile Photo', icon: <ImageIcon className="w-4 h-4 mr-2" /> },
  ] as const;

  if (loading && user?.memberId) {
    return (
      <div className="flex h-full items-center justify-center">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-indigo-500"></div>
      </div>
    );
  }

  const isLibrarianWithoutMemberProfile = user?.role === 'Librarian' && !user?.memberId;

  return (
    <div className="max-w-5xl mx-auto space-y-6">
      <div>
        <h1 className="text-2xl font-bold tracking-tight text-white">My Profile</h1>
        <p className="text-sm text-slate-400 mt-1">Manage your account settings and preferences.</p>
      </div>

      <div className="flex flex-col md:flex-row gap-6 items-start">
        {/* Sidebar Tabs */}
        <div className="w-full md:w-64 flex flex-col gap-1 bg-slate-900 border border-slate-800 rounded-xl p-2 shrink-0">
          {tabs.map((tab) => (
            <button
              key={tab.id}
              onClick={() => setActiveTab(tab.id)}
              className={`flex items-center w-full px-4 py-3 text-sm font-medium rounded-lg transition-colors ${
                activeTab === tab.id
                  ? 'bg-indigo-500/10 text-indigo-400'
                  : 'text-slate-400 hover:bg-slate-800 hover:text-slate-200'
              }`}
            >
              {tab.icon}
              {tab.label}
            </button>
          ))}
        </div>

        {/* Main Content Area */}
        <div className="flex-1 w-full bg-slate-900 border border-slate-800 rounded-xl overflow-hidden min-h-[400px]">
          
          {/* TAB 1: Personal Info */}
          {activeTab === 'personal' && (
            <div className="p-6">
              <h2 className="text-lg font-medium text-white mb-6">Personal Information</h2>
              {isLibrarianWithoutMemberProfile ? (
                <div className="bg-slate-800/50 p-6 rounded-lg border border-slate-700 text-center">
                  <UserCircle className="w-12 h-12 text-slate-500 mx-auto mb-3" />
                  <h3 className="text-white font-medium mb-1">Librarian Account</h3>
                  <p className="text-sm text-slate-400 max-w-md mx-auto">
                    Your account is currently set up as a Librarian without a linked member profile. To manage personal information, borrow books, or hold a library card, a Member profile must be created and linked to this account.
                  </p>
                  <div className="mt-4 pt-4 border-t border-slate-700 space-y-2 text-sm text-left max-w-sm mx-auto bg-slate-900 p-4 rounded-md">
                     <div className="flex justify-between"><span className="text-slate-500">Username:</span> <span className="text-slate-300 font-medium">{user?.username}</span></div>
                     <div className="flex justify-between"><span className="text-slate-500">Role:</span> <span className="text-slate-300 font-medium">{user?.role}</span></div>
                  </div>
                </div>
              ) : (
              <form onSubmit={handlePersonalSubmit(onPersonalSubmit)} className="space-y-4 max-w-xl">
                <div className="grid grid-cols-2 gap-4">
                  <div className="space-y-1">
                    <label className="text-sm font-medium text-slate-300">First Name</label>
                    <input {...regPersonal("firstName")} className="w-full px-3 py-2 bg-slate-950 border border-slate-700 rounded-lg text-white" />
                  </div>
                  <div className="space-y-1">
                    <label className="text-sm font-medium text-slate-300">Last Name</label>
                    <input {...regPersonal("lastName")} className="w-full px-3 py-2 bg-slate-950 border border-slate-700 rounded-lg text-white" />
                  </div>
                </div>
                <div className="space-y-1">
                  <label className="text-sm font-medium text-slate-300">Email Address (Read Only)</label>
                  <input value={memberInfo?.email || ''} readOnly disabled className="w-full px-3 py-2 bg-slate-950/50 border border-slate-800 rounded-lg text-slate-500 cursor-not-allowed" />
                </div>
                <div className="space-y-1">
                  <label className="text-sm font-medium text-slate-300">Phone Number</label>
                  <input {...regPersonal("phone")} className="w-full px-3 py-2 bg-slate-950 border border-slate-700 rounded-lg text-white" />
                </div>
                <div className="space-y-1">
                  <label className="text-sm font-medium text-slate-300">Address</label>
                  <textarea {...regPersonal("address")} rows={3} className="w-full px-3 py-2 bg-slate-950 border border-slate-700 rounded-lg text-white resize-none" />
                </div>
                <div className="pt-4 border-t border-slate-800">
                  <button type="submit" disabled={isPersonalSubmitting} className="px-4 py-2 bg-indigo-600 hover:bg-indigo-500 text-white rounded-lg disabled:opacity-50">
                    {isPersonalSubmitting ? 'Saving...' : 'Save Changes'}
                  </button>
                </div>
              </form>
              )}
            </div>
          )}

          {/* TAB 2: Security */}
          {activeTab === 'security' && (
            <div className="p-6">
              <h2 className="text-lg font-medium text-white mb-6">Security Settings</h2>
              
              <div className="max-w-xl space-y-8">
                <form onSubmit={handlePasswordSubmit(onPasswordSubmit)} className="space-y-4">
                  <h3 className="text-sm font-semibold text-slate-400 uppercase tracking-wider">Change Password</h3>
                  <div className="space-y-1">
                    <label className="text-sm font-medium text-slate-300">Current Password</label>
                    <input type="password" {...regPassword("currentPassword")} className="w-full px-3 py-2 bg-slate-950 border border-slate-700 rounded-lg text-white" />
                  </div>
                  <div className="space-y-1">
                    <label className="text-sm font-medium text-slate-300">New Password</label>
                    <input type="password" {...regPassword("newPassword")} className="w-full px-3 py-2 bg-slate-950 border border-slate-700 rounded-lg text-white" />
                  </div>
                  <div className="space-y-1">
                    <label className="text-sm font-medium text-slate-300">Confirm New Password</label>
                    <input type="password" {...regPassword("confirmPassword")} className="w-full px-3 py-2 bg-slate-950 border border-slate-700 rounded-lg text-white" />
                    {pwdErrors.confirmPassword && <p className="text-red-400 text-xs mt-1">{pwdErrors.confirmPassword.message}</p>}
                  </div>
                  <button type="submit" disabled={isPwdSubmitting} className="px-4 py-2 bg-indigo-600 hover:bg-indigo-500 text-white rounded-lg disabled:opacity-50 mt-2">
                    {isPwdSubmitting ? 'Updating...' : 'Update Password'}
                  </button>
                </form>

                <div className="pt-8 border-t border-slate-800">
                  <h3 className="text-sm font-semibold text-slate-400 uppercase tracking-wider mb-4">Change Email Address</h3>
                  <p className="text-sm text-slate-400 mb-4">To change your email address, an OTP will be sent to your new email for verification.</p>
                  <button disabled className="px-4 py-2 bg-slate-800 text-slate-500 rounded-lg cursor-not-allowed border border-slate-700">
                    Request Email Change (Coming Soon)
                  </button>
                </div>
              </div>
            </div>
          )}

          {/* TAB 3: Membership */}
          {activeTab === 'membership' && (
            <div className="p-6">
              <h2 className="text-lg font-medium text-white mb-6">Library Membership</h2>
              {isLibrarianWithoutMemberProfile ? (
                <div className="bg-slate-800/50 p-6 rounded-lg border border-slate-700 text-center">
                  <CreditCard className="w-12 h-12 text-slate-500 mx-auto mb-3" />
                  <h3 className="text-white font-medium mb-1">No Membership Found</h3>
                  <p className="text-sm text-slate-400 max-w-md mx-auto">
                    You do not have an active Member profile linked to this account.
                  </p>
                </div>
              ) : memberInfo ? (
                <div className="bg-slate-950 rounded-xl p-6 border border-slate-800 relative overflow-hidden max-w-xl">
                  <div className="absolute top-0 right-0 w-32 h-32 bg-indigo-500/10 rounded-bl-full -mr-4 -mt-4"></div>
                  <div className="space-y-6 relative">
                    <div>
                      <p className="text-sm text-slate-500">Membership ID</p>
                      <p className="text-xl font-mono font-bold text-white tracking-widest mt-1">{memberInfo?.membershipNumber}</p>
                    </div>
                    <div className="grid grid-cols-2 gap-4">
                      <div>
                        <p className="text-sm text-slate-500">Status</p>
                        <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium border mt-1 ${memberInfo?.status === "Active" ? "bg-emerald-500/10 text-emerald-400 border-emerald-500/20" : "bg-red-500/10 text-red-400 border-red-500/20"}`}>
                          {memberInfo?.status}
                        </span>
                      </div>
                      <div>
                        <p className="text-sm text-slate-500">Join Date</p>
                        <p className="text-slate-300 font-medium mt-1">{new Date(memberInfo?.joinDate || '').toLocaleDateString()}</p>
                      </div>
                    </div>
                    <div className="pt-6 border-t border-slate-800/50">
                      <p className="text-sm text-slate-400 mb-3">Your membership is valid for 1 year from the join date.</p>
                      <button disabled className="px-4 py-2 bg-indigo-500/50 text-indigo-200 rounded-lg cursor-not-allowed font-medium text-sm">
                        Renew Membership
                      </button>
                    </div>
                  </div>
                </div>
              ) : null}
            </div>
          )}

          {/* TAB 4: Borrow History */}
          {activeTab === 'history' && (
            <div className="p-6 flex flex-col h-full">
              <div className="flex justify-between items-center mb-6">
                <h2 className="text-lg font-medium text-white">Borrowing History</h2>
                {isLibrarianWithoutMemberProfile ? null : (
                  <select value={borrowStatus} onChange={(e) => { setBorrowStatus(e.target.value); setBorrowPage(1); }} className="px-3 py-1.5 bg-slate-950 border border-slate-700 rounded-lg text-slate-300 text-sm focus:outline-none">
                    <option value="">All Statuses</option>
                    <option value="Active">Active</option>
                    <option value="Returned">Returned</option>
                    <option value="Overdue">Overdue</option>
                  </select>
                )}
              </div>

              {isLibrarianWithoutMemberProfile ? (
                 <div className="flex-1 flex flex-col items-center justify-center p-12 text-center bg-slate-800/20 rounded-xl border border-slate-800">
                    <History className="w-12 h-12 text-slate-600 mb-4" />
                    <h3 className="text-slate-300 font-medium">No Borrowing Records</h3>
                    <p className="text-sm text-slate-500 max-w-xs mt-2">Member profiles are required to track borrowing history.</p>
                 </div>
              ) : (
                <>
                  <div className="overflow-x-auto border border-slate-800 rounded-lg flex-1">
                    <table className="w-full text-left text-sm text-slate-300">
                      <thead className="bg-slate-950 border-b border-slate-800 text-slate-400 text-xs uppercase">
                        <tr>
                          <th className="px-4 py-3">Book</th>
                          <th className="px-4 py-3">Borrowed On</th>
                          <th className="px-4 py-3">Due Date</th>
                          <th className="px-4 py-3">Status</th>
                        </tr>
                      </thead>
                      <tbody>
                        {!borrows || borrows.items.length === 0 ? (
                          <tr><td colSpan={4} className="px-4 py-8 text-center text-slate-500">No borrowing records found.</td></tr>
                        ) : (
                          borrows.items.map((b) => (
                            <tr key={b.id} className="border-b border-slate-800/50 last:border-0 hover:bg-slate-800/20">
                              <td className="px-4 py-3 font-medium text-slate-200">{b.bookTitle}</td>
                              <td className="px-4 py-3 text-slate-400">{new Date(b.borrowDate).toLocaleDateString()}</td>
                              <td className="px-4 py-3 text-slate-400">{new Date(b.dueDate).toLocaleDateString()}</td>
                              <td className="px-4 py-3">
                                <span className={`inline-flex items-center px-2 py-0.5 rounded text-[10px] font-medium border ${b.status === "Returned" ? "bg-slate-500/10 text-slate-400 border-slate-500/20" : b.isOverdue ? "bg-red-500/10 text-red-400 border-red-500/20" : "bg-emerald-500/10 text-emerald-400 border-emerald-500/20"}`}>
                                  {b.isOverdue && b.status === "Active" ? "Overdue" : b.status}
                                </span>
                              </td>
                            </tr>
                          ))
                        )}
                      </tbody>
                    </table>
                  </div>
                  
                  {/* Pagination */}
                  {borrows && borrows.totalPages > 1 && (
                    <div className="mt-4 pt-4 border-t border-slate-800 flex justify-between items-center text-sm shrink-0">
                      <span className="text-slate-400">Page {borrowPage} of {borrows.totalPages}</span>
                      <div className="flex gap-2">
                        <button onClick={() => setBorrowPage(p => Math.max(1, p - 1))} disabled={!borrows.hasPreviousPage} className="px-3 py-1.5 bg-slate-800 text-slate-300 rounded hover:bg-slate-700 disabled:opacity-50">Previous</button>
                        <button onClick={() => setBorrowPage(p => Math.min(borrows.totalPages, p + 1))} disabled={!borrows.hasNextPage} className="px-3 py-1.5 bg-slate-800 text-slate-300 rounded hover:bg-slate-700 disabled:opacity-50">Next</button>
                      </div>
                    </div>
                  )}
                </>
              )}
            </div>
          )}

          {/* TAB 5: Photo */}
          {activeTab === 'photo' && (
            <div className="p-6">
              <h2 className="text-lg font-medium text-white mb-6">Profile Photo</h2>
              {isLibrarianWithoutMemberProfile ? (
                <div className="bg-slate-800/50 p-6 rounded-lg border border-slate-700 text-center">
                  <ImageIcon className="w-12 h-12 text-slate-500 mx-auto mb-3" />
                  <h3 className="text-white font-medium mb-1">No Profile Photo</h3>
                  <p className="text-sm text-slate-400 max-w-md mx-auto">
                    Profile photos are associated with Member accounts.
                  </p>
                </div>
              ) : (
              <div className="flex flex-col items-center justify-center p-8 border-2 border-dashed border-slate-700 rounded-xl bg-slate-900/50">
                <div className="w-24 h-24 mx-auto bg-slate-800 rounded-full flex items-center justify-center mb-4">
                  <UserCircle className="w-12 h-12 text-slate-500" />
                </div>
                <h3 className="text-slate-300 font-medium mb-1">Upload a new photo</h3>
                <p className="text-sm text-slate-500 mb-6">JPG, GIF or PNG. Max size of 2MB.</p>
                <button disabled className="px-6 py-2 bg-slate-800 text-slate-500 border border-slate-700 rounded-lg cursor-not-allowed">
                  Select File (Coming Soon)
                </button>
              </div>
              )}
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
