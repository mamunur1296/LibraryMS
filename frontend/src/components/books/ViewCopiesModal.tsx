import { useState, useEffect, useMemo } from "react";
import { bookService } from "@/lib/services/book.service";
import { BookDto, BookCopy } from "@/types/book.types";
import { toast } from "@/components/ui/Toast";

interface ViewCopiesModalProps {
  isOpen: boolean;
  onClose: () => void;
  book: BookDto | null;
}

export function ViewCopiesModal({ isOpen, onClose, book }: ViewCopiesModalProps) {
  const [copies, setCopies] = useState<BookCopy[]>([]);
  const [loading, setLoading] = useState(false);
  const [filterStatus, setFilterStatus] = useState<string>("All");

  useEffect(() => {
    if (isOpen && book) {
      setFilterStatus("All");
      fetchCopies();
    }
  }, [isOpen, book]);

  const fetchCopies = async () => {
    if (!book) return;
    setLoading(true);
    try {
      const data = await bookService.getBookCopies(book.id);
      setCopies(data);
    } catch (error) {
      toast.error("Failed to load book copies");
    } finally {
      setLoading(false);
    }
  };

  const branchSummaries = useMemo(() => {
    const summaryMap: Record<string, { branchName: string, total: number, available: number, borrowed: number, damaged: number, lost: number }> = {};
    
    copies.forEach(copy => {
      const branchName = copy.branchName || "Unknown Branch";
      if (!summaryMap[branchName]) {
        summaryMap[branchName] = { branchName, total: 0, available: 0, borrowed: 0, damaged: 0, lost: 0 };
      }
      summaryMap[branchName].total += 1;
      if (copy.status === "Available") summaryMap[branchName].available += 1;
      if (copy.status === "Borrowed") summaryMap[branchName].borrowed += 1;
      if (copy.status === "Damaged") summaryMap[branchName].damaged += 1;
      if (copy.status === "Lost") summaryMap[branchName].lost += 1;
    });
  
    return Object.values(summaryMap);
  }, [copies]);

  if (!isOpen || !book) return null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/60 backdrop-blur-sm overflow-y-auto pt-10 pb-10">
      <div className="w-full max-w-4xl bg-slate-900 border border-slate-700 rounded-2xl shadow-2xl overflow-hidden m-4 flex flex-col max-h-[85vh]">
        
        {/* Header */}
        <div className="flex items-center justify-between px-6 py-4 border-b border-slate-800 bg-slate-900/50 shrink-0">
          <div>
            <h2 className="text-xl font-semibold text-white flex items-center gap-2">
              <svg className="w-5 h-5 text-indigo-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" />
              </svg>
              Branch Availability: {book.title}
            </h2>
            <p className="text-sm text-slate-400 mt-1">Total Copies Across All Branches: {copies.length}</p>
          </div>
          <button onClick={onClose} className="text-slate-400 hover:text-white transition-colors p-2 hover:bg-slate-800 rounded-full">
            <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        {/* Content */}
        <div className="p-0 overflow-y-auto min-h-[300px] relative bg-slate-950/30">
          {loading ? (
            <div className="absolute inset-0 flex justify-center items-center bg-slate-900/50 backdrop-blur-sm z-10">
              <div className="flex flex-col items-center gap-3">
                <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-indigo-500"></div>
                <span className="text-sm text-slate-400 animate-pulse">Loading branches...</span>
              </div>
            </div>
          ) : branchSummaries.length === 0 ? (
            <div className="flex flex-col items-center justify-center h-full min-h-[300px] text-slate-500">
              <svg className="w-16 h-16 mb-4 text-slate-700" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10" />
              </svg>
              <p className="text-lg font-medium text-slate-400">No copies found.</p>
              <p className="text-sm mt-1">This book has not been added to any branch yet.</p>
            </div>
          ) : (
            <table className="w-full text-left border-collapse">
              <thead className="sticky top-0 bg-slate-900/95 backdrop-blur-sm border-b border-slate-800 shadow-sm z-10">
                <tr>
                  <th className="px-6 py-4 text-xs font-semibold tracking-wider text-slate-400 uppercase">Branch Name</th>
                  <th className="px-6 py-4 text-xs font-semibold tracking-wider text-slate-400 uppercase text-center">Total Copies</th>
                  <th className="px-6 py-4 text-xs font-semibold tracking-wider text-emerald-400 uppercase text-center">Available</th>
                  <th className="px-6 py-4 text-xs font-semibold tracking-wider text-amber-400 uppercase text-center">Borrowed</th>
                  <th className="px-6 py-4 text-xs font-semibold tracking-wider text-red-400 uppercase text-center">Damaged/Lost</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-800/60">
                {branchSummaries.map((summary, idx) => (
                  <tr key={idx} className="hover:bg-slate-800/40 transition-colors group">
                    <td className="px-6 py-4">
                      <div className="flex items-center gap-3">
                        <div className="w-10 h-10 rounded-xl bg-indigo-500/10 border border-indigo-500/20 flex items-center justify-center text-indigo-400 group-hover:scale-110 transition-transform">
                          <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" />
                          </svg>
                        </div>
                        <span className="text-base font-medium text-white">
                          {summary.branchName}
                        </span>
                      </div>
                    </td>
                    <td className="px-6 py-4 text-center">
                      <span className="text-lg font-bold text-slate-300 bg-slate-800/50 px-3 py-1 rounded-lg border border-slate-700/50">
                        {summary.total}
                      </span>
                    </td>
                    <td className="px-6 py-4 text-center">
                      <span className={`text-lg font-bold ${summary.available > 0 ? "text-emerald-400" : "text-slate-600"}`}>
                        {summary.available}
                      </span>
                    </td>
                    <td className="px-6 py-4 text-center">
                      <span className={`text-lg font-bold ${summary.borrowed > 0 ? "text-amber-400" : "text-slate-600"}`}>
                        {summary.borrowed}
                      </span>
                    </td>
                    <td className="px-6 py-4 text-center">
                      <span className={`text-lg font-bold ${(summary.damaged + summary.lost) > 0 ? "text-red-400" : "text-slate-600"}`}>
                        {summary.damaged + summary.lost}
                      </span>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>
      </div>
    </div>
  );
}
