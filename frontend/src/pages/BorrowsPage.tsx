import { useEffect, useState, useCallback } from "react";
import { useSearchParams } from "react-router-dom";
import { borrowService } from "@/lib/services/borrow.service";
import { authService } from "@/lib/services/auth.service";
import { BorrowDto, PagedResult } from "@/types/borrow.types";
import { BorrowFormModal } from "@/components/borrows/BorrowFormModal";
import { ReturnBookModal } from "@/components/borrows/ReturnBookModal";
import { ConfirmDialog } from "@/components/ui/ConfirmDialog";
import { toast } from "@/components/ui/Toast";

import { useAuth } from "@/contexts/AuthContext";

export default function BorrowsPage() {
  const { user } = useAuth();
  const [searchParams, setSearchParams] = useSearchParams();
  const [data, setData] = useState<PagedResult<BorrowDto> | null>(null);
  const [loading, setLoading] = useState(true);
  const [statusFilter, setStatusFilter] = useState("");
  const [page, setPage] = useState(1);
  const [isFormModalOpen, setIsFormModalOpen] = useState(false);
  const [isReturnModalOpen, setIsReturnModalOpen] = useState(false);
  const [borrowToManage, setBorrowToManage] = useState<BorrowDto | null>(null);
  const [finePayTarget, setFinePayTarget] = useState<BorrowDto | null>(null);

  const userRole = authService.getUserRole();
  const isLibrarianOrAdmin = userRole === "Librarian" || userRole === "Admin";

  const fetchBorrows = useCallback(async () => {
    setLoading(true);
    try {
      const targetMemberId = !isLibrarianOrAdmin ? (user?.memberId ?? undefined) : undefined;
      const result = await borrowService.search(targetMemberId, undefined, statusFilter || undefined, page, 10);
      setData(result);
    } catch {
      toast.error("Failed to fetch borrow records.");
    } finally {
      setLoading(false);
    }
  }, [statusFilter, page]);

  useEffect(() => { void fetchBorrows(); }, [fetchBorrows]);

  useEffect(() => {
    if (searchParams.get("action") === "new") {
      setIsFormModalOpen(true);
      // Remove the action from URL so it doesn't reopen on refresh if not desired, 
      // but keeping it is fine as the modal close will just close it.
    }
  }, [searchParams]);
  const handlePayFineConfirm = async () => {
    if (!finePayTarget) return;
    try {
      await borrowService.payFine(finePayTarget.id);
      toast.success("Fine payment processed successfully.");
      void fetchBorrows();
    } catch (error: unknown) {
      const msg = error instanceof Error ? error.message : "Failed to process payment.";
      toast.error(msg);
    } finally {
      setFinePayTarget(null);
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold tracking-tight text-white">{isLibrarianOrAdmin ? 'Borrow & Return' : 'My Borrows'}</h1>
          <p className="text-sm text-slate-400 mt-1">{isLibrarianOrAdmin ? 'Issue books, process returns, and manage fines.' : 'View your borrowing history and due dates.'}</p>
        </div>
        {isLibrarianOrAdmin && (
          <button onClick={() => { setIsFormModalOpen(true); }} className="px-4 py-2 bg-indigo-600 hover:bg-indigo-500 text-white rounded-lg shadow-lg shadow-indigo-500/20 text-sm font-medium transition-all flex items-center gap-2">
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 6v6m0 0v6m0-6h6m-6 0H6" /></svg>
            Issue Book
          </button>
        )}
      </div>

      <div className="bg-slate-900 border border-slate-800 rounded-2xl overflow-hidden shadow-sm flex flex-col">
        <div className="p-4 border-b border-slate-800 flex flex-col sm:flex-row justify-between bg-slate-900/50">
          <select value={statusFilter} onChange={(e) => { setStatusFilter(e.target.value); setPage(1); }} className="px-4 py-2 bg-slate-950 border border-slate-700 rounded-lg text-slate-300 focus:outline-none focus:ring-2 focus:ring-indigo-500 text-sm min-w-[200px]">
            <option value="">All Statuses</option>
            <option value="Active">Active</option>
            <option value="Overdue">Overdue</option>
            <option value="Returned">Returned</option>
          </select>
        </div>

        <div className="overflow-x-auto">
          <table className="w-full text-left text-sm text-slate-300">
            <thead className="text-xs uppercase bg-slate-900 border-b border-slate-800 text-slate-400">
              <tr>
                <th className="px-6 py-4 font-medium">Book Details</th>
                {isLibrarianOrAdmin && <th className="px-6 py-4 font-medium">Member</th>}
                <th className="px-6 py-4 font-medium">Timeline</th>
                <th className="px-6 py-4 font-medium">Status / Fines</th>
                <th className="px-6 py-4 text-right font-medium">Actions</th>
              </tr>
            </thead>
            <tbody>
              {loading ? (
                <tr><td colSpan={5} className="px-6 py-8 text-center"><div className="flex items-center justify-center"><div className="animate-spin rounded-full h-8 w-8 border-b-2 border-indigo-500"></div></div></td></tr>
              ) : !data || data.items.length === 0 ? (
                <tr><td colSpan={5} className="px-6 py-8 text-center text-slate-500">No borrowing records found.</td></tr>
              ) : (
                data.items.map((borrow) => (
                  <tr key={borrow.id} className="border-b border-slate-800/50 hover:bg-slate-800/20 transition-colors">
                    <td className="px-6 py-4">
                      <div className="font-medium text-white">{borrow.bookTitle}</div>
                      <div className="text-xs text-slate-500 mt-0.5 font-mono">Copy: #{borrow.copyNumber}</div>
                    </td>
                    {isLibrarianOrAdmin && (
                      <td className="px-6 py-4">
                        <div className="text-slate-300">{borrow.memberName}</div>
                        <div className="text-xs text-slate-500 mt-0.5">ID: {borrow.membershipNumber}</div>
                      </td>
                    )}
                    <td className="px-6 py-4">
                      <div className="text-xs text-slate-400">Borrowed: {new Date(borrow.borrowDate).toLocaleDateString()}</div>
                      <div className={`text-xs mt-1 ${borrow.isOverdue && borrow.status === "Active" ? "text-red-400 font-medium" : "text-slate-400"}`}>Due: {new Date(borrow.dueDate).toLocaleDateString()}</div>
                    </td>
                    <td className="px-6 py-4">
                      <div className="flex flex-col gap-2 items-start">
                        <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium border ${borrow.status === "Returned" ? "bg-slate-500/10 text-slate-400 border-slate-500/20" : borrow.isOverdue ? "bg-red-500/10 text-red-400 border-red-500/20" : "bg-emerald-500/10 text-emerald-400 border-emerald-500/20"}`}>
                          {borrow.isOverdue && borrow.status === "Active" ? "Overdue" : borrow.status}
                        </span>
                        {borrow.lateFine > 0 && (
                          <span className={`text-xs font-semibold ${borrow.isFinePaid ? "text-emerald-500" : "text-red-400"}`}>
                            Fine: ${borrow.lateFine.toFixed(2)} {borrow.isFinePaid ? "(Paid)" : "(Unpaid)"}
                          </span>
                        )}
                      </div>
                    </td>
                    <td className="px-6 py-4 text-right space-x-3 whitespace-nowrap">
                      {isLibrarianOrAdmin ? (
                        <>
                          {borrow.status === "Active" && (
                            <button onClick={() => { setBorrowToManage(borrow); setIsReturnModalOpen(true); }} className="px-3 py-1.5 bg-indigo-500/10 hover:bg-indigo-500/20 text-indigo-400 rounded text-xs font-medium">Process Return</button>
                          )}
                          {borrow.lateFine > 0 && !borrow.isFinePaid && (
                            <button onClick={() => { setFinePayTarget(borrow); }} className="px-3 py-1.5 bg-emerald-500/10 hover:bg-emerald-500/20 text-emerald-400 rounded text-xs font-medium">Pay Fine</button>
                          )}
                        </>
                      ) : (
                        <span className="text-slate-500 text-xs italic">View Only</span>
                      )}
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
              <button onClick={() => { setPage((p) => Math.max(1, p - 1)); }} disabled={!data.hasPreviousPage} className="px-3 py-1 bg-slate-800 text-slate-300 rounded hover:bg-slate-700 disabled:opacity-50 text-sm">Previous</button>
              <button onClick={() => { setPage((p) => Math.min(data.totalPages, p + 1)); }} disabled={!data.hasNextPage} className="px-3 py-1 bg-slate-800 text-slate-300 rounded hover:bg-slate-700 disabled:opacity-50 text-sm">Next</button>
            </div>
          </div>
        )}
      </div>

      <BorrowFormModal 
        isOpen={isFormModalOpen} 
        onClose={() => { 
          setIsFormModalOpen(false);
          if (searchParams.get("action") === "new") {
            searchParams.delete("action");
            searchParams.delete("bookId");
            setSearchParams(searchParams);
          }
        }} 
        onSuccess={() => { void fetchBorrows(); }} 
      />
      <ReturnBookModal isOpen={isReturnModalOpen} onClose={() => { setIsReturnModalOpen(false); }} onSuccess={() => { void fetchBorrows(); }} borrow={borrowToManage} />

      <ConfirmDialog
        isOpen={finePayTarget !== null}
        title="Confirm Fine Payment"
        message={`Confirm payment of $${finePayTarget?.lateFine.toFixed(2)} fine for "${finePayTarget?.bookTitle}"?`}
        confirmText="Confirm Payment"
        variant="warning"
        onConfirm={() => { void handlePayFineConfirm(); }}
        onCancel={() => { setFinePayTarget(null); }}
      />
    </div>
  );
}
