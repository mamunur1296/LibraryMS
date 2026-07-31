import { useEffect, useState, useCallback, useMemo } from 'react';
import { useAuth } from '@/contexts/AuthContext';
import { borrowService } from '@/lib/services/borrow.service';
import type { BorrowDto } from '@/types/borrow.types';
import { toast } from '@/components/ui/Toast';
import { ConfirmDialog } from '@/components/ui/ConfirmDialog';
import { CreditCard, Banknote, AlertCircle, History } from 'lucide-react';

export default function MyFinesPage() {
  const { user } = useAuth();
  const [loading, setLoading] = useState(true);
  const [allBorrows, setAllBorrows] = useState<BorrowDto[]>([]);
  const [payTarget, setPayTarget] = useState<BorrowDto | null>(null);

  // We fetch all records for the user and then filter them on the client side since we don't have a specific fine API
  const fetchFinesData = useCallback(async () => {
    if (!user?.memberId) return;
    setLoading(true);
    try {
      // In a real app, we'd have a specific endpoint or pagination handling. 
      // For this demo, we fetch a large page size to get all records that might have fines.
      const result = await borrowService.search(user.memberId, undefined, undefined, 1, 100);
      setAllBorrows(result.items.filter(b => b.lateFine > 0));
    } catch {
      toast.error('Failed to load fines data');
    } finally {
      setLoading(false);
    }
  }, [user]);

  useEffect(() => {
    void fetchFinesData();
  }, [fetchFinesData]);

  const unpaidFines = useMemo(() => allBorrows.filter(b => !b.isFinePaid), [allBorrows]);
  const paidFines = useMemo(() => allBorrows.filter(b => b.isFinePaid), [allBorrows]);
  const totalOutstanding = useMemo(() => unpaidFines.reduce((sum, b) => sum + b.lateFine, 0), [unpaidFines]);

  const handlePayConfirm = async () => {
    if (!payTarget) return;
    try {
      await borrowService.payFine(payTarget.id);
      toast.success('Fine marked as paid.');
      void fetchFinesData();
    } catch (error: unknown) {
      const msg = error instanceof Error ? error.message : "Failed to process payment.";
      toast.error(msg);
    } finally {
      setPayTarget(null);
    }
  };

  if (loading) {
    return (
      <div className="flex h-full items-center justify-center">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-indigo-500"></div>
      </div>
    );
  }

  return (
    <div className="max-w-6xl mx-auto space-y-6">
      <div>
        <h1 className="text-2xl font-bold tracking-tight text-white">Fines & Payments</h1>
        <p className="text-sm text-slate-400 mt-1">Manage your library fines and view payment history.</p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        {/* Outstanding Summary Card */}
        <div className="bg-slate-900 border border-slate-800 rounded-2xl p-6 relative overflow-hidden flex flex-col justify-between">
          <div className="absolute top-0 right-0 p-6 opacity-10">
            <AlertCircle className="w-24 h-24 text-amber-500" />
          </div>
          <div className="relative z-10">
            <h2 className="text-slate-400 font-medium">Total Outstanding Fines</h2>
            <p className={`text-4xl font-bold mt-2 ${totalOutstanding > 0 ? 'text-amber-500' : 'text-emerald-500'}`}>
              ${totalOutstanding.toFixed(2)}
            </p>
          </div>
          
          <div className="mt-6 relative z-10">
            <button disabled className="w-full py-3 px-4 bg-indigo-600/50 text-indigo-300 font-medium rounded-xl border border-indigo-500/30 cursor-not-allowed flex items-center justify-center gap-2 transition-all">
              <CreditCard className="w-5 h-5" />
              Pay All Online (Coming Soon)
            </button>
            <p className="text-xs text-slate-500 text-center mt-3">
              Online payments are currently disabled. Please pay at the counter.
            </p>
          </div>
        </div>

        {/* Unpaid Fines Breakdown */}
        <div className="md:col-span-2 bg-slate-900 border border-slate-800 rounded-2xl flex flex-col">
          <div className="p-6 border-b border-slate-800 flex items-center justify-between">
            <h2 className="text-lg font-medium text-white flex items-center gap-2">
              <Banknote className="w-5 h-5 text-amber-400" />
              Pending Payments
            </h2>
          </div>
          <div className="overflow-x-auto flex-1">
            <table className="w-full text-left text-sm text-slate-300">
              <thead className="bg-slate-950 border-b border-slate-800 text-slate-400 text-xs uppercase">
                <tr>
                  <th className="px-6 py-4 font-medium">Book Details</th>
                  <th className="px-6 py-4 font-medium">Overdue Details</th>
                  <th className="px-6 py-4 font-medium text-right">Amount</th>
                  <th className="px-6 py-4 font-medium text-center">Action</th>
                </tr>
              </thead>
              <tbody>
                {unpaidFines.length === 0 ? (
                  <tr>
                    <td colSpan={4} className="px-6 py-12 text-center">
                      <div className="flex flex-col items-center justify-center">
                        <div className="w-12 h-12 rounded-full bg-emerald-500/10 flex items-center justify-center mb-3">
                          <svg className="w-6 h-6 text-emerald-400" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" /></svg>
                        </div>
                        <p className="text-slate-400">You have no pending fines. Great job!</p>
                      </div>
                    </td>
                  </tr>
                ) : (
                  unpaidFines.map((fine) => (
                    <tr key={fine.id} className="border-b border-slate-800/50 hover:bg-slate-800/20">
                      <td className="px-6 py-4">
                        <div className="font-medium text-white">{fine.bookTitle}</div>
                        <div className="text-xs text-slate-500">Borrowed: {new Date(fine.borrowDate).toLocaleDateString()}</div>
                      </td>
                      <td className="px-6 py-4">
                        <div className="text-amber-400/90 text-sm">Due: {new Date(fine.dueDate).toLocaleDateString()}</div>
                        <div className="text-xs text-slate-500">{fine.status === 'Returned' ? 'Returned late' : 'Currently overdue'}</div>
                      </td>
                      <td className="px-6 py-4 text-right">
                        <span className="font-bold text-amber-500">${fine.lateFine.toFixed(2)}</span>
                      </td>
                      <td className="px-6 py-4 text-center">
                        <button 
                          onClick={() => setPayTarget(fine)}
                          className="px-3 py-1.5 bg-slate-800 hover:bg-slate-700 text-slate-200 text-xs font-medium rounded transition-colors border border-slate-700"
                        >
                          Pay in Person
                        </button>
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>
        </div>
      </div>

      {/* Payment History */}
      <div className="bg-slate-900 border border-slate-800 rounded-2xl flex flex-col">
        <div className="p-6 border-b border-slate-800 flex items-center justify-between">
          <h2 className="text-lg font-medium text-white flex items-center gap-2">
            <History className="w-5 h-5 text-indigo-400" />
            Payment History
          </h2>
        </div>
        <div className="overflow-x-auto">
          <table className="w-full text-left text-sm text-slate-300">
            <thead className="bg-slate-950 border-b border-slate-800 text-slate-400 text-xs uppercase">
              <tr>
                <th className="px-6 py-4 font-medium">Book Details</th>
                <th className="px-6 py-4 font-medium">Fine Reason</th>
                <th className="px-6 py-4 font-medium text-right">Amount Paid</th>
                <th className="px-6 py-4 font-medium text-center">Status</th>
              </tr>
            </thead>
            <tbody>
              {paidFines.length === 0 ? (
                <tr>
                  <td colSpan={4} className="px-6 py-8 text-center text-slate-500">
                    No payment history found.
                  </td>
                </tr>
              ) : (
                paidFines.map((fine) => (
                  <tr key={fine.id} className="border-b border-slate-800/50 hover:bg-slate-800/20">
                    <td className="px-6 py-4">
                      <div className="font-medium text-slate-200">{fine.bookTitle}</div>
                    </td>
                    <td className="px-6 py-4 text-slate-400">
                      Late return
                    </td>
                    <td className="px-6 py-4 text-right">
                      <span className="font-medium text-slate-200">${fine.lateFine.toFixed(2)}</span>
                    </td>
                    <td className="px-6 py-4 text-center">
                      <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-[10px] font-medium bg-emerald-500/10 text-emerald-400 border border-emerald-500/20">
                        Paid
                      </span>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </div>

      <ConfirmDialog
        isOpen={payTarget !== null}
        title="Confirm In-Person Payment"
        message={`Are you confirming that you are paying the $${payTarget?.lateFine.toFixed(2)} fine for "${payTarget?.bookTitle}" at the library counter?`}
        confirmText="Confirm Payment"
        variant="warning"
        onConfirm={() => { void handlePayConfirm(); }}
        onCancel={() => { setPayTarget(null); }}
      />
    </div>
  );
}
