import { useEffect, useState } from 'react';
import { branchService } from '@/lib/services/branch.service';
import { BranchDto } from '@/types/branch.types';
import { BranchFormModal } from '@/components/branches/BranchFormModal';

export default function BranchesPage() {
  const [branches, setBranches] = useState<BranchDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [branchToEdit, setBranchToEdit] = useState<BranchDto | null>(null);

  const fetchBranches = async () => {
    setLoading(true);
    try { const data = await branchService.getAll(true); setBranches(data); }
    catch (error) { console.error('Failed to fetch branches', error); }
    finally { setLoading(false); }
  };

  useEffect(() => { fetchBranches(); }, []);

  const handleToggleStatus = async (branch: BranchDto) => {
    try {
      if (branch.isActive) { await branchService.deactivate(branch.id); }
      else { await branchService.activate(branch.id); }
      fetchBranches();
    } catch { alert('Failed to change status.'); }
  };

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold tracking-tight text-white">Branch Management</h1>
          <p className="text-sm text-slate-400 mt-1">Manage all library branches across different locations.</p>
        </div>
        <button onClick={() => { setBranchToEdit(null); setIsModalOpen(true); }} className="px-4 py-2 bg-indigo-600 hover:bg-indigo-500 text-white rounded-lg shadow-lg shadow-indigo-500/20 text-sm font-medium transition-all flex items-center gap-2">
          <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 6v6m0 0v6m0-6h6m-6 0H6" /></svg>
          Add Branch
        </button>
      </div>

      <div className="bg-slate-900 border border-slate-800 rounded-2xl overflow-hidden shadow-sm">
        <div className="overflow-x-auto">
          <table className="w-full text-left text-sm text-slate-300">
            <thead className="text-xs uppercase bg-slate-900 border-b border-slate-800 text-slate-400">
              <tr>
                <th className="px-6 py-4 font-medium">Branch Name</th>
                <th className="px-6 py-4 font-medium">Contact</th>
                <th className="px-6 py-4 font-medium">Status</th>
                <th className="px-6 py-4 font-medium">Created At</th>
                <th className="px-6 py-4 text-right font-medium">Actions</th>
              </tr>
            </thead>
            <tbody>
              {loading ? (
                <tr><td colSpan={5} className="px-6 py-8 text-center"><div className="flex items-center justify-center"><div className="animate-spin rounded-full h-8 w-8 border-b-2 border-indigo-500"></div></div></td></tr>
              ) : branches.length === 0 ? (
                <tr><td colSpan={5} className="px-6 py-8 text-center text-slate-500">No branches found. Click "Add Branch" to create one.</td></tr>
              ) : (
                branches.map((branch) => (
                  <tr key={branch.id} className="border-b border-slate-800/50 hover:bg-slate-800/20 transition-colors">
                    <td className="px-6 py-4">
                      <div className="font-medium text-white">{branch.name}</div>
                      <div className="text-xs text-slate-500 mt-0.5 truncate max-w-[200px]">{branch.address}</div>
                    </td>
                    <td className="px-6 py-4">
                      <div className="text-slate-300">{branch.email}</div>
                      <div className="text-xs text-slate-500 mt-0.5">{branch.phone}</div>
                    </td>
                    <td className="px-6 py-4">
                      <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium border ${branch.isActive ? 'bg-emerald-500/10 text-emerald-400 border-emerald-500/20' : 'bg-red-500/10 text-red-400 border-red-500/20'}`}>
                        {branch.isActive ? 'Active' : 'Inactive'}
                      </span>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-slate-400">{new Date(branch.createdAt).toLocaleDateString()}</td>
                    <td className="px-6 py-4 text-right space-x-3 whitespace-nowrap">
                      <button onClick={() => handleToggleStatus(branch)} className={`text-xs font-medium hover:underline ${branch.isActive ? 'text-amber-500' : 'text-emerald-500'}`}>{branch.isActive ? 'Deactivate' : 'Activate'}</button>
                      <button onClick={() => { setBranchToEdit(branch); setIsModalOpen(true); }} className="text-indigo-400 hover:text-indigo-300 text-xs font-medium">Edit</button>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </div>

      <BranchFormModal isOpen={isModalOpen} onClose={() => setIsModalOpen(false)} onSuccess={fetchBranches} branchToEdit={branchToEdit} />
    </div>
  );
}
