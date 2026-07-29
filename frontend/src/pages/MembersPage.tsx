import { useEffect, useState } from 'react';
import { memberService } from '@/lib/services/member.service';
import { MemberDto, PagedResult } from '@/types/member.types';
import { MemberFormModal } from '@/components/members/MemberFormModal';
import { SuspendMemberModal } from '@/components/members/SuspendMemberModal';

export default function MembersPage() {
  const [data, setData] = useState<PagedResult<MemberDto> | null>(null);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState('');
  const [page, setPage] = useState(1);
  const [isFormModalOpen, setIsFormModalOpen] = useState(false);
  const [isSuspendModalOpen, setIsSuspendModalOpen] = useState(false);
  const [memberToManage, setMemberToManage] = useState<MemberDto | null>(null);

  const fetchMembers = async () => {
    setLoading(true);
    try {
      const result = await memberService.search(searchTerm, statusFilter || undefined, page, 10);
      setData(result);
    } catch (error) {
      console.error('Failed to fetch members', error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    const delayDebounceFn = setTimeout(() => { fetchMembers(); }, 300);
    return () => clearTimeout(delayDebounceFn);
  }, [searchTerm, statusFilter, page]);

  const handleActivate = async (id: string) => {
    if (confirm('Are you sure you want to reactivate this member?')) {
      try { await memberService.activate(id); fetchMembers(); }
      catch { alert('Failed to activate member.'); }
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold tracking-tight text-white">Member Management</h1>
          <p className="text-sm text-slate-400 mt-1">Manage library members, statuses, and accounts.</p>
        </div>
        <button onClick={() => { setMemberToManage(null); setIsFormModalOpen(true); }} className="px-4 py-2 bg-indigo-600 hover:bg-indigo-500 text-white rounded-lg shadow-lg shadow-indigo-500/20 text-sm font-medium transition-all flex items-center gap-2">
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
              {loading && !data ? (
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
                      <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium border ${member.status === 'Active' ? 'bg-emerald-500/10 text-emerald-400 border-emerald-500/20' : member.status === 'Suspended' ? 'bg-amber-500/10 text-amber-400 border-amber-500/20' : 'bg-red-500/10 text-red-400 border-red-500/20'}`}>
                        {member.status}
                      </span>
                      {member.status === 'Suspended' && member.suspendedUntil && (
                        <div className="text-[10px] text-amber-500/70 mt-1">Until: {new Date(member.suspendedUntil).toLocaleDateString()}</div>
                      )}
                    </td>
                    <td className="px-6 py-4 text-center">
                      <span className={`inline-flex items-center justify-center w-8 h-8 rounded-full ${member.activeBorrows > 0 ? 'bg-indigo-500/20 text-indigo-400 font-bold' : 'bg-slate-800 text-slate-500'}`}>{member.activeBorrows}</span>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-slate-400">{new Date(member.joinDate).toLocaleDateString()}</td>
                    <td className="px-6 py-4 text-right space-x-3 whitespace-nowrap">
                      {member.status === 'Active' ? (
                        <button onClick={() => { setMemberToManage(member); setIsSuspendModalOpen(true); }} className="text-amber-400 hover:text-amber-300 text-xs font-medium">Suspend</button>
                      ) : (
                        <button onClick={() => handleActivate(member.id)} className="text-emerald-400 hover:text-emerald-300 text-xs font-medium">Activate</button>
                      )}
                      <button onClick={() => { setMemberToManage(member); setIsFormModalOpen(true); }} className="text-indigo-400 hover:text-indigo-300 text-xs font-medium">Edit</button>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>

        {data && data.totalPages > 1 && (
          <div className="px-6 py-3 border-t border-slate-800 bg-slate-900/50 flex items-center justify-between">
            <div className="text-sm text-slate-400">Showing <span className="font-medium text-white">{(page - 1) * 10 + 1}</span> to <span className="font-medium text-white">{Math.min(page * 10, data.totalCount)}</span> of <span className="font-medium text-white">{data.totalCount}</span> results</div>
            <div className="flex space-x-2">
              <button onClick={() => setPage(p => Math.max(1, p - 1))} disabled={!data.hasPreviousPage} className="px-3 py-1 bg-slate-800 text-slate-300 rounded hover:bg-slate-700 disabled:opacity-50 text-sm">Previous</button>
              <button onClick={() => setPage(p => Math.min(data.totalPages, p + 1))} disabled={!data.hasNextPage} className="px-3 py-1 bg-slate-800 text-slate-300 rounded hover:bg-slate-700 disabled:opacity-50 text-sm">Next</button>
            </div>
          </div>
        )}
      </div>

      <MemberFormModal isOpen={isFormModalOpen} onClose={() => setIsFormModalOpen(false)} onSuccess={fetchMembers} memberToEdit={memberToManage} />
      <SuspendMemberModal isOpen={isSuspendModalOpen} onClose={() => setIsSuspendModalOpen(false)} onSuccess={fetchMembers} member={memberToManage} />
    </div>
  );
}
