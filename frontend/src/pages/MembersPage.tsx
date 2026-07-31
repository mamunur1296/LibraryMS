import { useEffect, useState, useCallback } from "react";
import { memberService } from "@/lib/services/member.service";
import { MemberDto, PagedResult } from "@/types/member.types";
import { MemberFormModal } from "@/components/members/MemberFormModal";
import { SuspendMemberModal } from "@/components/members/SuspendMemberModal";
import { ResetPasswordModal } from "@/components/members/ResetPasswordModal";
import { MemberStatsModal } from "@/components/members/MemberStatsModal";
import { MemberProfileModal } from "@/components/members/MemberProfileModal";
import { LibraryCardModal } from "@/components/members/LibraryCardModal";
import { RenewMembershipModal } from "@/components/members/RenewMembershipModal";
import { ConfirmDialog } from "@/components/ui/ConfirmDialog";
import { toast } from "@/components/ui/Toast";

export default function MembersPage() {
  const [data, setData] = useState<PagedResult<MemberDto> | null>(null);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState("");
  const [statusFilter, setStatusFilter] = useState("");
  const [page, setPage] = useState(1);
  const [isFormModalOpen, setIsFormModalOpen] = useState(false);
  const [isSuspendModalOpen, setIsSuspendModalOpen] = useState(false);
  const [isResetPasswordOpen, setIsResetPasswordOpen] = useState(false);
  const [isStatsOpen, setIsStatsOpen] = useState(false);
  const [isProfileOpen, setIsProfileOpen] = useState(false);
  const [isCardOpen, setIsCardOpen] = useState(false);
  const [isRenewOpen, setIsRenewOpen] = useState(false);
  const [memberToManage, setMemberToManage] = useState<MemberDto | null>(null);
  const [activateTarget, setActivateTarget] = useState<MemberDto | null>(null);

  const fetchMembers = useCallback(async () => {
    setLoading(true);
    try {
      const result = await memberService.search(searchTerm, statusFilter || undefined, page, 10);
      setData(result);
    } catch {
      toast.error("Failed to fetch members.");
    } finally {
      setLoading(false);
    }
  }, [searchTerm, statusFilter, page]);

  useEffect(() => {
    const delay = setTimeout(() => { void fetchMembers(); }, 300);
    return () => { clearTimeout(delay); };
  }, [fetchMembers]);

  const handleActivateConfirm = async () => {
    if (!activateTarget) return;
    try {
      await memberService.activate(activateTarget.id);
      toast.success(`${activateTarget.fullName} has been reactivated.`);
      void fetchMembers();
    } catch {
      toast.error("Failed to activate member.");
    } finally {
      setActivateTarget(null);
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold tracking-tight text-white">Member Management</h1>
          <p className="text-sm text-slate-400 mt-1">Manage library members, statuses, and accounts.</p>
        </div>
        <button
          onClick={() => { setMemberToManage(null); setIsFormModalOpen(true); }}
          className="px-4 py-2 bg-indigo-600 hover:bg-indigo-500 text-white rounded-lg shadow-lg shadow-indigo-500/20 text-sm font-medium transition-all flex items-center gap-2"
        >
          <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 6v6m0 0v6m0-6h6m-6 0H6" /></svg>
          Add Member
        </button>
      </div>

      <div className="bg-slate-900 border border-slate-800 rounded-2xl overflow-hidden shadow-sm flex flex-col">
        <div className="p-4 border-b border-slate-800 flex flex-col sm:flex-row gap-4 justify-between bg-slate-900/50">
          <div className="relative w-full max-w-md">
            <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
              <svg className="h-5 w-5 text-slate-500" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" /></svg>
            </div>
            <input type="text" placeholder="Search members by name, email, or ID..." value={searchTerm} onChange={(e) => { setSearchTerm(e.target.value); setPage(1); }} className="block w-full pl-10 pr-3 py-2 border border-slate-700 rounded-lg bg-slate-950 text-slate-300 placeholder-slate-500 focus:outline-none focus:ring-1 focus:ring-indigo-500 sm:text-sm" />
          </div>
          <select value={statusFilter} onChange={(e) => { setStatusFilter(e.target.value); setPage(1); }} className="px-4 py-2 bg-slate-950 border border-slate-700 rounded-lg text-slate-300 focus:outline-none focus:ring-2 focus:ring-indigo-500 text-sm min-w-[150px]">
            <option value="">All Statuses</option>
            <option value="Active">Active</option>
            <option value="Suspended">Suspended</option>
            <option value="Inactive">Inactive</option>
          </select>
        </div>

        <div className="overflow-x-auto">
          <table className="w-full text-left text-sm text-slate-300">
            <thead className="text-xs uppercase bg-slate-900 border-b border-slate-800 text-slate-400">
              <tr>
                <th className="px-6 py-4 font-medium">Member Info</th>
                <th className="px-6 py-4 font-medium">Contact</th>
                <th className="px-6 py-4 font-medium">Status</th>
                <th className="px-6 py-4 font-medium text-center">Active Borrows</th>
                <th className="px-6 py-4 font-medium">Join Date</th>
                <th className="px-6 py-4 text-right font-medium">Actions</th>
              </tr>
            </thead>
            <tbody>
              {loading ? (
                <tr><td colSpan={6} className="px-6 py-8 text-center"><div className="flex items-center justify-center"><div className="animate-spin rounded-full h-8 w-8 border-b-2 border-indigo-500"></div></div></td></tr>
              ) : !data || data.items.length === 0 ? (
                <tr><td colSpan={6} className="px-6 py-8 text-center text-slate-500">No members found matching your criteria.</td></tr>
              ) : (
                data.items.map((member) => (
                  <tr key={member.id} className="border-b border-slate-800/50 hover:bg-slate-800/20 transition-colors">
                    <td className="px-6 py-4">
                      <div className="font-medium text-white">{member.fullName}</div>
                      <div className="text-xs text-slate-500 mt-0.5 font-mono">ID: {member.membershipNumber}</div>
                    </td>
                    <td className="px-6 py-4">
                      <div className="text-slate-300">{member.email}</div>
                      <div className="text-xs text-slate-500 mt-0.5">{member.phone}</div>
                    </td>
                    <td className="px-6 py-4">
                      <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium border ${member.status === "Active" ? "bg-emerald-500/10 text-emerald-400 border-emerald-500/20" : member.status === "Suspended" ? "bg-amber-500/10 text-amber-400 border-amber-500/20" : "bg-red-500/10 text-red-400 border-red-500/20"}`}>
                        {member.status}
                      </span>
                      {member.status === "Suspended" && member.suspendedUntil && (
                        <div className="text-[10px] text-amber-500/70 mt-1">Until: {new Date(member.suspendedUntil).toLocaleDateString()}</div>
                      )}
                    </td>
                    <td className="px-6 py-4 text-center">
                      <span className={`inline-flex items-center justify-center w-8 h-8 rounded-full ${member.activeBorrows > 0 ? "bg-indigo-500/20 text-indigo-400 font-bold" : "bg-slate-800 text-slate-500"}`}>{member.activeBorrows}</span>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-slate-400">{new Date(member.joinDate).toLocaleDateString()}</td>
                    <td className="px-6 py-4 text-right space-x-2 whitespace-nowrap">
                      <button onClick={() => { setMemberToManage(member); setIsProfileOpen(true); }} className="text-indigo-400 hover:text-indigo-300 text-xs font-medium">Profile</button>
                      <button onClick={() => { setMemberToManage(member); setIsCardOpen(true); }} className="text-purple-400 hover:text-purple-300 text-xs font-medium">Card</button>
                      
                      <div className="inline-block border-l border-slate-700 mx-2 h-3 align-middle"></div>
                      
                      <button onClick={() => { setMemberToManage(member); setIsStatsOpen(true); }} className="text-teal-400 hover:text-teal-350 text-xs font-medium">Stats</button>
                      <button onClick={() => { setMemberToManage(member); setIsFormModalOpen(true); }} className="text-indigo-400 hover:text-indigo-300 text-xs font-medium">Edit</button>
                      <button onClick={() => { setMemberToManage(member); setIsRenewOpen(true); }} className="text-emerald-400 hover:text-emerald-300 text-xs font-medium">Renew</button>
                      <button onClick={() => { setMemberToManage(member); setIsResetPasswordOpen(true); }} className={`${member.hasAccount ? "text-rose-450 hover:text-rose-455" : "text-sky-405 hover:text-sky-400"} text-xs font-medium`}>
                        {member.hasAccount ? "Reset PW" : "Create Acc"}
                      </button>
                      {member.status === "Active" ? (
                        <button onClick={() => { setMemberToManage(member); setIsSuspendModalOpen(true); }} className="text-amber-400 hover:text-amber-300 text-xs font-medium">Suspend</button>
                      ) : (
                        <button onClick={() => { setActivateTarget(member); }} className="text-emerald-400 hover:text-emerald-300 text-xs font-medium">Activate</button>
                      )}
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>

        {data && (
          <div className="px-6 py-3 border-t border-slate-800 bg-slate-900/50 flex items-center justify-between">
            <div className="text-sm text-slate-400">
              Showing <span className="font-medium text-white">{data.totalCount === 0 ? 0 : (page - 1) * 10 + 1}</span> to{" "}
              <span className="font-medium text-white">{Math.min(page * 10, data.totalCount)}</span> of{" "}
              <span className="font-medium text-white">{data.totalCount}</span> results
            </div>
            <div className="flex space-x-1 items-center">
              <button onClick={() => { setPage((p) => Math.max(1, p - 1)); }} disabled={!data.hasPreviousPage} className="px-3 py-1 bg-slate-800 text-slate-300 rounded hover:bg-slate-700 disabled:opacity-50 text-sm border border-slate-700">Previous</button>
              
              <div className="hidden sm:flex space-x-1 mx-1">
                {Array.from({ length: Math.max(1, data.totalPages) }).map((_, i) => (
                  <button
                    key={i + 1}
                    onClick={() => setPage(i + 1)}
                    className={`px-3 py-1 rounded text-sm border ${
                      page === i + 1 
                        ? "bg-indigo-600 text-white border-indigo-500 shadow-sm shadow-indigo-500/20" 
                        : "bg-slate-800 text-slate-300 border-slate-700 hover:bg-slate-700"
                    }`}
                  >
                    {i + 1}
                  </button>
                ))}
              </div>

              <button onClick={() => { setPage((p) => Math.min(Math.max(1, data.totalPages), p + 1)); }} disabled={!data.hasNextPage} className="px-3 py-1 bg-slate-800 text-slate-300 rounded hover:bg-slate-700 disabled:opacity-50 text-sm border border-slate-700">Next</button>
            </div>
          </div>
        )}
      </div>

      <MemberFormModal isOpen={isFormModalOpen} onClose={() => { setIsFormModalOpen(false); }} onSuccess={() => { void fetchMembers(); }} memberToEdit={memberToManage} />
      <SuspendMemberModal isOpen={isSuspendModalOpen} onClose={() => { setIsSuspendModalOpen(false); }} onSuccess={() => { void fetchMembers(); }} member={memberToManage} />
      <ResetPasswordModal isOpen={isResetPasswordOpen} onClose={() => { setIsResetPasswordOpen(false); }} onSuccess={() => { void fetchMembers(); }} member={memberToManage} />
      <MemberStatsModal isOpen={isStatsOpen} onClose={() => { setIsStatsOpen(false); }} member={memberToManage} />
      <MemberProfileModal isOpen={isProfileOpen} onClose={() => { setIsProfileOpen(false); }} member={memberToManage} />
      <LibraryCardModal isOpen={isCardOpen} onClose={() => { setIsCardOpen(false); }} member={memberToManage} />
      <RenewMembershipModal isOpen={isRenewOpen} onClose={() => { setIsRenewOpen(false); }} onSuccess={() => { void fetchMembers(); }} member={memberToManage} />

      <ConfirmDialog
        isOpen={activateTarget !== null}
        title="Reactivate Member"
        message={`Are you sure you want to reactivate ${activateTarget?.fullName}?`}
        confirmText="Reactivate"
        variant="default"
        onConfirm={() => { void handleActivateConfirm(); }}
        onCancel={() => { setActivateTarget(null); }}
      />
    </div>
  );
}
