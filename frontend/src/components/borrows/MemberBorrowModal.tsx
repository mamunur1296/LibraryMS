import { useEffect, useState } from "react";
import { borrowService } from "@/lib/services/borrow.service";
import { BookDto, BookCopy } from "@/types/book.types";
import { toast } from "@/components/ui/Toast";
import { apiClient } from "@/lib/api-client";

interface MemberBorrowModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
  book: BookDto | null;
  branchId: string;
  branchName: string;
  memberId: string;
}

export function MemberBorrowModal({
  isOpen,
  onClose,
  onSuccess,
  book,
  branchId,
  branchName,
  memberId,
}: MemberBorrowModalProps) {
  const [borrowDays, setBorrowDays] = useState(14);
  const [selectedCopy, setSelectedCopy] = useState<BookCopy | null>(null);
  const [loadingCopies, setLoadingCopies] = useState(false);
  const [submitting, setSubmitting] = useState(false);

  // Fetch available copies and auto-select the first one whenever the modal opens
  useEffect(() => {
    if (!isOpen || !book) return;

    setBorrowDays(14);
    setSelectedCopy(null);

    const fetchCopy = async () => {
      setLoadingCopies(true);
      try {
        // Fetch available copies for this book, filtered by selected branch if one is chosen
        const url = branchId
          ? `/api/Books/${book.id}/available-copies?branchId=${branchId}`
          : `/api/Books/${book.id}/available-copies`;
        const res = await apiClient.get<BookCopy[]>(url);
        const copies: BookCopy[] = res.data;
        // Pick the first available copy in the chosen branch, or just the first one
        const copy = copies.find((c) => !branchId || c.branchId === branchId) ?? copies[0] ?? null;
        setSelectedCopy(copy);
      } catch {
        toast.error("Could not load available copies.");
      } finally {
        setLoadingCopies(false);
      }
    };

    void fetchCopy();
  }, [isOpen, book, branchId]);

  if (!isOpen || !book) return null;

  const dueDate = new Date();
  dueDate.setDate(dueDate.getDate() + borrowDays);

  const handleBorrow = async () => {
    if (!selectedCopy) {
      toast.error("No available copy found.");
      return;
    }
    setSubmitting(true);
    try {
      await borrowService.borrowBook({
        memberId,
        bookId: book.id,
        bookCopyId: selectedCopy.id,
        branchId: selectedCopy.branchId,
        borrowDays,
      });
      toast.success(`"${book.title}" borrowed successfully! Due: ${dueDate.toLocaleDateString()}`);
      onSuccess();
      onClose();
    } catch (err: unknown) {
      const axiosErr = err as { response?: { data?: { message?: string } }; message?: string };
      toast.error(axiosErr.response?.data?.message ?? axiosErr.message ?? "Failed to borrow book.");
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/60 backdrop-blur-sm px-4">
      <div className="w-full max-w-md bg-slate-900 border border-slate-700 rounded-2xl shadow-2xl overflow-hidden">

        {/* Header */}
        <div className="flex items-center justify-between px-6 py-4 border-b border-slate-800 bg-slate-900/50">
          <div className="flex items-center gap-3">
            <div className="w-8 h-8 rounded-lg bg-indigo-500/20 flex items-center justify-center">
              <svg className="w-4 h-4 text-indigo-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 6.253v13m0-13C10.832 5.477 9.246 5 7.5 5S4.168 5.477 3 6.253v13C4.168 18.477 5.754 18 7.5 18s3.332.477 4.5 1.253m0-13C13.168 5.477 14.754 5 16.5 5c1.747 0 3.332.477 4.5 1.253v13C19.832 18.477 18.247 18 16.5 18c-1.746 0-3.332.477-4.5 1.253" />
              </svg>
            </div>
            <h2 className="text-lg font-semibold text-white">Borrow Book</h2>
          </div>
          <button onClick={onClose} className="text-slate-400 hover:text-white transition-colors p-1 rounded-lg hover:bg-slate-800">
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <div className="p-6 space-y-5">

          {/* Book info card */}
          <div className="p-4 bg-slate-800/60 rounded-xl border border-slate-700/50">
            <p className="text-xs text-slate-400 mb-1 uppercase tracking-wide font-medium">Book</p>
            <p className="text-white font-semibold text-base leading-tight">{book.title}</p>
            <p className="text-slate-400 text-sm mt-0.5">by {book.authorName}</p>
            <p className="text-slate-500 text-xs mt-1 font-mono">ISBN: {book.isbn}</p>
          </div>

          {/* Branch + Copy info */}
          <div className="grid grid-cols-2 gap-3">
            <div className="p-3 bg-slate-800/40 rounded-xl border border-slate-700/40">
              <p className="text-xs text-slate-400 mb-1 uppercase tracking-wide font-medium">Branch</p>
              {branchName ? (
                <p className="text-slate-200 text-sm font-medium">{branchName}</p>
              ) : (
                <p className="text-slate-500 text-sm italic">Any branch</p>
              )}
            </div>
            <div className="p-3 bg-slate-800/40 rounded-xl border border-slate-700/40">
              <p className="text-xs text-slate-400 mb-1 uppercase tracking-wide font-medium">Copy #</p>
              {loadingCopies ? (
                <div className="flex items-center gap-2">
                  <div className="w-3 h-3 rounded-full border border-indigo-400 border-t-transparent animate-spin" />
                  <span className="text-slate-500 text-xs">Loading…</span>
                </div>
              ) : selectedCopy ? (
                <p className="text-emerald-400 text-sm font-semibold">#{selectedCopy.copyNumber}</p>
              ) : (
                <p className="text-red-400 text-sm">None available</p>
              )}
            </div>
          </div>

          {/* Borrow Days */}
          <div>
            <label className="block text-sm font-medium text-slate-300 mb-2">
              How many days do you want to borrow? (Max 14 days)
            </label>
            <div className="flex items-center gap-3">
              {/* Quick presets */}
              {[7, 14].map((d) => (
                <button
                  key={d}
                  type="button"
                  onClick={() => setBorrowDays(d)}
                  className={`flex-1 py-2 rounded-lg text-sm font-medium transition-all border ${
                    borrowDays === d
                      ? "bg-indigo-600 text-white border-indigo-500 shadow-sm shadow-indigo-500/30"
                      : "bg-slate-800 text-slate-400 border-slate-700 hover:border-slate-600 hover:text-slate-200"
                  }`}
                >
                  {d}d
                </button>
              ))}
            </div>
            <div className="flex items-center gap-2 mt-3">
              <label className="text-xs text-slate-500 whitespace-nowrap">Custom days:</label>
              <input
                type="number"
                min={1}
                max={14}
                value={borrowDays}
                onChange={(e) => setBorrowDays(Math.max(1, Math.min(14, Number(e.target.value))))}
                className="w-24 px-3 py-1.5 bg-slate-950 border border-slate-700 rounded-lg text-white text-sm focus:outline-none focus:ring-1 focus:ring-indigo-500 focus:border-indigo-500"
              />
            </div>
          </div>

          {/* Due date preview */}
          <div className="flex items-center gap-2 p-3 bg-indigo-500/5 border border-indigo-500/20 rounded-xl">
            <svg className="w-4 h-4 text-indigo-400 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
            </svg>
            <span className="text-sm text-slate-300">
              Due date:{" "}
              <span className="text-indigo-300 font-semibold">
                {dueDate.toLocaleDateString("en-GB", { day: "numeric", month: "short", year: "numeric" })}
              </span>
            </span>
          </div>

          {/* Actions */}
          <div className="flex justify-end gap-3 pt-2 border-t border-slate-800">
            <button
              type="button"
              onClick={onClose}
              className="px-4 py-2 rounded-lg text-sm font-medium text-slate-300 hover:text-white hover:bg-slate-800 transition-colors"
            >
              Cancel
            </button>
            <button
              type="button"
              onClick={() => { void handleBorrow(); }}
              disabled={submitting || loadingCopies || !selectedCopy}
              className="px-5 py-2 bg-indigo-600 hover:bg-indigo-500 text-white rounded-lg text-sm font-semibold transition-all shadow-lg shadow-indigo-500/20 disabled:opacity-50 disabled:cursor-not-allowed flex items-center gap-2"
            >
              {submitting && (
                <span className="w-4 h-4 border-2 border-white/20 border-t-white rounded-full animate-spin" />
              )}
              {submitting ? "Borrowing…" : "Confirm Borrow"}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
