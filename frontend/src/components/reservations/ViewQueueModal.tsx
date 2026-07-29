import { useEffect, useState } from "react";
import { reservationService } from "@/lib/services/reservation.service";
import { ReservationQueueDto } from "@/types/reservation.types";

interface ViewQueueModalProps {
  isOpen: boolean;
  onClose: () => void;
  bookId: string | null;
  branchId: string | null;
}

export function ViewQueueModal({ isOpen, onClose, bookId, branchId }: ViewQueueModalProps) {
  const [queueData, setQueueData] = useState<ReservationQueueDto | null>(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (isOpen && bookId && branchId) {
      fetchQueue();
    } else {
      setQueueData(null);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [isOpen, bookId, branchId]);

  const fetchQueue = async () => {
    setLoading(true);
    try {
      const data = await reservationService.getQueue(bookId!, branchId!);
      setQueueData(data);
    } catch (error) {
      console.error("Failed to fetch queue", error);
    } finally {
      setLoading(false);
    }
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 z-[60] flex items-center justify-center bg-black/60 backdrop-blur-sm">
      <div className="w-full max-w-2xl bg-slate-800 border border-slate-600 rounded-xl shadow-2xl overflow-hidden flex flex-col max-h-[80vh]">
        <div className="flex items-center justify-between px-6 py-4 border-b border-slate-700 bg-slate-800">
          <div>
            <h3 className="text-lg font-semibold text-white flex items-center gap-2">
              <svg className="w-5 h-5 text-indigo-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
              </svg>
              Reservation Waitlist
            </h3>
            {queueData && (
              <p className="text-sm text-slate-400 mt-1">
                {queueData.bookTitle} &bull; {queueData.branchName}
              </p>
            )}
          </div>
          <button type="button" onClick={onClose} className="text-slate-400 hover:text-white transition-colors">
            <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <div className="p-6 overflow-y-auto flex-1">
          {loading ? (
             <div className="flex justify-center py-10">
               <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-indigo-500"></div>
             </div>
          ) : !queueData ? (
             <div className="text-center py-10 text-slate-400">Failed to load queue data.</div>
          ) : (
             <div className="space-y-4">
               <div className="flex justify-between items-center bg-slate-900/50 p-4 rounded-lg border border-slate-700">
                 <span className="text-sm font-medium text-slate-300">Total Members Waiting</span>
                 <span className="text-lg font-bold text-indigo-400">{queueData.totalInQueue}</span>
               </div>

               {queueData.queue.length === 0 ? (
                 <div className="text-center py-8 text-slate-500 bg-slate-900/30 rounded-lg border border-dashed border-slate-700">
                   No one is currently waiting for this book.
                 </div>
               ) : (
                 <div className="space-y-2">
                   {queueData.queue.map((reservation, index) => (
                     <div 
                        key={reservation.id} 
                        className={`flex items-center justify-between p-4 rounded-lg border ${
                          index === 0 
                            ? "bg-emerald-500/10 border-emerald-500/20" 
                            : "bg-slate-900/50 border-slate-700"
                        }`}
                     >
                       <div className="flex items-center gap-4">
                         <div className={`flex items-center justify-center w-8 h-8 rounded-full font-bold text-sm ${
                           index === 0 ? "bg-emerald-500 text-white" : "bg-slate-800 text-slate-400"
                         }`}>
                           {reservation.queuePosition}
                         </div>
                         <div>
                           <div className="font-medium text-white">{reservation.memberName}</div>
                           <div className="text-xs text-slate-400 mt-0.5">ID: {reservation.membershipNumber} &bull; Reserved: {new Date(reservation.createdAt).toLocaleDateString()}</div>
                         </div>
                       </div>
                       
                       <div className="text-right">
                         <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium border ${
                            reservation.status === "Notified"
                              ? "bg-indigo-500/10 text-indigo-400 border-indigo-500/20" 
                              : "bg-slate-800 text-slate-400 border-slate-700"
                          }`}>
                            {reservation.status}
                          </span>
                       </div>
                     </div>
                   ))}
                 </div>
               )}
             </div>
          )}
        </div>
      </div>
    </div>
  );
}
