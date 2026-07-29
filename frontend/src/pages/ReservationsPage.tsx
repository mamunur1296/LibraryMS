import { useEffect, useState, useCallback } from "react";
import { reservationService } from "@/lib/services/reservation.service";
import { ReservationDto, PagedResult } from "@/types/reservation.types";
import { ReservationFormModal } from "@/components/reservations/ReservationFormModal";
import { ViewQueueModal } from "@/components/reservations/ViewQueueModal";
import { ConfirmDialog } from "@/components/ui/ConfirmDialog";
import { toast } from "@/components/ui/Toast";

export default function ReservationsPage() {
  const [data, setData] = useState<PagedResult<ReservationDto> | null>(null);
  const [loading, setLoading] = useState(true);
  const [statusFilter, setStatusFilter] = useState("");
  const [page, setPage] = useState(1);
  const [isFormModalOpen, setIsFormModalOpen] = useState(false);
  const [isQueueModalOpen, setIsQueueModalOpen] = useState(false);
  const [selectedBookId, setSelectedBookId] = useState<string | null>(null);
  const [selectedBranchId, setSelectedBranchId] = useState<string | null>(null);
  const [cancelTarget, setCancelTarget] = useState<ReservationDto | null>(null);

  const fetchReservations = useCallback(async () => {
    setLoading(true);
    try {
      const result = await reservationService.search(undefined, undefined, statusFilter || undefined, page, 10);
      setData(result);
    } catch {
      toast.error("Failed to fetch reservations.");
    } finally {
      setLoading(false);
    }
  }, [statusFilter, page]);

  useEffect(() => { void fetchReservations(); }, [fetchReservations]);

  const handleCancelConfirm = async () => {
    if (!cancelTarget) return;
    try {
      await reservationService.cancel(cancelTarget.id);
      toast.success("Reservation cancelled successfully.");
      void fetchReservations();
    } catch (error: unknown) {
      const msg = error instanceof Error ? error.message : "Failed to cancel reservation.";
      toast.error(msg);
    } finally {
      setCancelTarget(null);
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold tracking-tight text-white">Reservation Queue</h1>
          <p className="text-sm text-slate-400 mt-1">Manage book reservations, waitlists, and holds.</p>
        </div>
        <button onClick={() => { setIsFormModalOpen(true); }} className="px-4 py-2 bg-indigo-600 hover:bg-indigo-500 text-white rounded-lg shadow-lg shadow-indigo-500/20 text-sm font-medium transition-all flex items-center gap-2">
          <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 6v6m0 0v6m0-6h6m-6 0H6" /></svg>
          Place Reservation
        </button>
      </div>

      <div className="bg-slate-900 border border-slate-800 rounded-2xl overflow-hidden shadow-sm flex flex-col">
        <div className="p-4 border-b border-slate-800 flex flex-col sm:flex-row justify-between bg-slate-900/50">
          <select value={statusFilter} onChange={(e) => { setStatusFilter(e.target.value); setPage(1); }} className="px-4 py-2 bg-slate-950 border border-slate-700 rounded-lg text-slate-300 focus:outline-none focus:ring-2 focus:ring-indigo-500 text-sm min-w-[200px]">
            <option value="">All Statuses</option>
            <option value="Pending">Pending</option>
            <option value="Notified">Notified</option>
            <option value="Fulfilled">Fulfilled</option>
            <option value="Cancelled">Cancelled</option>
            <option value="Expired">Expired</option>
          </select>
        </div>

        <div className="overflow-x-auto">
          <table className="w-full text-left text-sm text-slate-300">
            <thead className="text-xs uppercase bg-slate-900 border-b border-slate-800 text-slate-400">
              <tr>
                <th className="px-6 py-4 font-medium">Book &amp; Branch</th>
                <th className="px-6 py-4 font-medium">Member</th>
                <th className="px-6 py-4 font-medium text-center">Queue Position</th>
                <th className="px-6 py-4 font-medium">Status &amp; Timeline</th>
                <th className="px-6 py-4 text-right font-medium">Actions</th>
              </tr>
            </thead>
            <tbody>
              {loading ? (
                <tr><td colSpan={5} className="px-6 py-8 text-center"><div className="flex items-center justify-center"><div className="animate-spin rounded-full h-8 w-8 border-b-2 border-indigo-500"></div></div></td></tr>
              ) : !data || data.items.length === 0 ? (
                <tr><td colSpan={5} className="px-6 py-8 text-center text-slate-500">No reservations found.</td></tr>
              ) : (
                data.items.map((reservation) => (
                  <tr key={reservation.id} className="border-b border-slate-800/50 hover:bg-slate-800/20 transition-colors">
                    <td className="px-6 py-4">
                      <div className="font-medium text-white">{reservation.bookTitle}</div>
                      <div className="text-xs text-slate-500 mt-0.5">Branch: {reservation.branchName}</div>
                    </td>
                    <td className="px-6 py-4">
                      <div className="text-slate-300">{reservation.memberName}</div>
                      <div className="text-xs text-slate-500 mt-0.5">ID: {reservation.membershipNumber}</div>
                    </td>
                    <td className="px-6 py-4 text-center">
                      {(reservation.status === "Pending" || reservation.status === "Notified") ? (
                        <span className="inline-flex flex-col items-center justify-center">
                          <span className="text-lg font-bold text-indigo-400">{reservation.queuePosition}</span>
                          <button onClick={() => { setSelectedBookId(reservation.bookId); setSelectedBranchId(reservation.branchId); setIsQueueModalOpen(true); }} className="text-[10px] text-slate-400 hover:text-indigo-300 transition-colors mt-1 underline underline-offset-2">View full queue</button>
                        </span>
                      ) : (<span className="text-slate-500">-</span>)}
                    </td>
                    <td className="px-6 py-4">
                      <div className="flex flex-col items-start gap-1">
                        <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium border ${reservation.status === "Pending" ? "bg-amber-500/10 text-amber-400 border-amber-500/20" : reservation.status === "Notified" ? "bg-blue-500/10 text-blue-400 border-blue-500/20" : reservation.status === "Fulfilled" ? "bg-emerald-500/10 text-emerald-400 border-emerald-500/20" : "bg-slate-500/10 text-slate-400 border-slate-500/20"}`}>{reservation.status}</span>
                        <span className="text-[10px] text-slate-500 mt-1">Placed: {new Date(reservation.createdAt).toLocaleDateString()}</span>
                      </div>
                    </td>
                    <td className="px-6 py-4 text-right whitespace-nowrap">
                      {(reservation.status === "Pending" || reservation.status === "Notified") && (
                        <button onClick={() => { setCancelTarget(reservation); }} className="px-3 py-1.5 bg-red-500/10 hover:bg-red-500/20 text-red-400 rounded text-xs font-medium">Cancel</button>
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

      <ReservationFormModal isOpen={isFormModalOpen} onClose={() => { setIsFormModalOpen(false); }} onSuccess={() => { void fetchReservations(); }} />
      <ViewQueueModal isOpen={isQueueModalOpen} onClose={() => { setIsQueueModalOpen(false); }} bookId={selectedBookId} branchId={selectedBranchId} />

      <ConfirmDialog
        isOpen={cancelTarget !== null}
        title="Cancel Reservation"
        message={`Are you sure you want to cancel the reservation for "${cancelTarget?.bookTitle}"?`}
        confirmText="Cancel Reservation"
        variant="danger"
        onConfirm={() => { void handleCancelConfirm(); }}
        onCancel={() => { setCancelTarget(null); }}
      />
    </div>
  );
}
