import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { returnBookSchema, ReturnBookFormData } from "@/lib/validations/borrow.schema";
import { borrowService } from "@/lib/services/borrow.service";
import { BorrowDto } from "@/types/borrow.types";

interface ReturnBookModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
  borrow: BorrowDto | null;
}

export function ReturnBookModal({ isOpen, onClose, onSuccess, borrow }: ReturnBookModalProps) {
  const {
    register,
    handleSubmit,
    reset,
    watch,
    formState: { errors, isSubmitting },
  } = useForm<ReturnBookFormData>({
    resolver: zodResolver(returnBookSchema),
    defaultValues: {
      fineCollection: "none"
    }
  });

  if (!isOpen || !borrow) return null;

  const fineCollection = watch("fineCollection");

  const onSubmit = async (data: ReturnBookFormData) => {
    try {
      await borrowService.returnBook({
        borrowId: borrow.id,
        notes: data.notes,
      });

      if (borrow.lateFine > 0 && data.fineCollection === "cash") {
        await borrowService.payFine(borrow.id);
      } else if (borrow.lateFine > 0 && data.fineCollection === "waive") {
        // Mock waive api call
        console.log("Waived fine for", borrow.id);
      }

      reset();
      
      // Mock checking for reservations
      const hasReservation = Math.random() > 0.5; // 50% chance to show for demonstration
      if (hasReservation) {
        alert(`Book returned successfully.\n\nSystem Note: "Next in queue has been notified."`);
      } else {
        alert("Book returned successfully.");
      }

      onSuccess();
      onClose();
    } catch (err: any) {
      alert(err.response?.data?.message || "Failed to return book");
    }
  };

  return (
    <div className="fixed inset-0 z-[60] flex items-center justify-center bg-black/60 backdrop-blur-sm">
      <div className="w-full max-w-md bg-slate-800 border border-slate-600 rounded-xl shadow-2xl overflow-hidden">
        <div className="flex items-center justify-between px-5 py-4 border-b border-slate-700 bg-slate-800">
          <h3 className="text-lg font-medium text-white">
            Return Book
          </h3>
          <button type="button" onClick={onClose} className="text-slate-400 hover:text-white transition-colors">
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <div className="p-5">
          <div className="mb-4 bg-slate-900/50 p-4 rounded-xl border border-slate-700">
            <h4 className="text-sm font-semibold text-white mb-2">Borrow Details</h4>
            <div className="grid grid-cols-2 gap-2 text-sm">
              <div className="text-slate-400">Book:</div>
              <div className="text-white font-medium truncate">{borrow.bookTitle}</div>
              <div className="text-slate-400">Member:</div>
              <div className="text-white font-medium">{borrow.memberName}</div>
              <div className="text-slate-400">Due Date:</div>
              <div className="text-white font-medium">{new Date(borrow.dueDate).toLocaleDateString()}</div>
            </div>
            
            {borrow.isOverdue && (
              <div className="mt-4 p-3 bg-red-500/10 border border-red-500/30 rounded-lg">
                <div className="flex justify-between items-center mb-1">
                  <span className="text-sm font-medium text-red-400 flex items-center gap-1.5">
                    <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" /></svg>
                    Overdue by {Math.abs(borrow.daysUntilDue)} days
                  </span>
                  <span className="text-lg font-bold text-red-400">${borrow.lateFine.toFixed(2)}</span>
                </div>
                <p className="text-xs text-red-400/80">Late fine has been applied to this borrow.</p>
              </div>
            )}
          </div>

          <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
            {borrow.isOverdue && borrow.lateFine > 0 && !borrow.isFinePaid && (
              <div className="space-y-2">
                <label className="block text-sm font-medium text-slate-300">Fine Collection</label>
                <div className="grid grid-cols-2 gap-2">
                  <label className={`flex items-center p-3 border rounded-lg cursor-pointer transition-colors ${fineCollection === 'none' ? 'bg-slate-800 border-slate-600' : 'bg-slate-900 border-slate-700 opacity-70'}`}>
                    <input type="radio" value="none" {...register("fineCollection")} className="hidden" />
                    <span className="text-sm font-medium text-slate-300">Collect Later</span>
                  </label>
                  <label className={`flex items-center p-3 border rounded-lg cursor-pointer transition-colors ${fineCollection === 'cash' ? 'bg-emerald-500/10 border-emerald-500/50' : 'bg-slate-900 border-slate-700 opacity-70'}`}>
                    <input type="radio" value="cash" {...register("fineCollection")} className="hidden" />
                    <span className="text-sm font-medium text-emerald-400">Cash (Counter)</span>
                  </label>
                  <label className={`flex items-center p-3 border rounded-lg cursor-pointer transition-colors ${fineCollection === 'waive' ? 'bg-amber-500/10 border-amber-500/50' : 'bg-slate-900 border-slate-700 opacity-70'}`}>
                    <input type="radio" value="waive" {...register("fineCollection")} className="hidden" />
                    <span className="text-sm font-medium text-amber-400">Waive Fine</span>
                  </label>
                  <label className="flex items-center justify-between p-3 border border-slate-700 bg-slate-900/50 rounded-lg cursor-not-allowed opacity-50" title="Member must pay online via portal">
                    <span className="text-sm font-medium text-slate-400">Online</span>
                    <svg className="w-4 h-4 text-slate-500" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z" /></svg>
                  </label>
                </div>
              </div>
            )}

            <div>
              <label className="block text-sm font-medium text-slate-300 mb-1">Return Notes (Optional)</label>
              <textarea
                {...register("notes")}
                rows={3}
                className="w-full px-3 py-2 bg-slate-900 border border-slate-700 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-indigo-500 resize-none"
                placeholder="Any condition issues, comments..."
              ></textarea>
              {errors.notes && <p className="text-red-400 text-xs mt-1">{errors.notes.message}</p>}
            </div>

            <div className="flex justify-end space-x-3 pt-4 border-t border-slate-700">
              <button
                type="button"
                onClick={onClose}
                className="px-3 py-1.5 rounded-lg text-sm font-medium text-slate-300 hover:text-white hover:bg-slate-700 transition-colors"
              >
                Cancel
              </button>
              <button
                type="submit"
                disabled={isSubmitting}
                className="px-4 py-2 bg-indigo-600 hover:bg-indigo-500 text-white rounded-lg text-sm font-medium transition-colors shadow-lg shadow-indigo-500/20 disabled:opacity-50 flex items-center"
              >
                {isSubmitting ? (
                   <span className="mr-2 border-2 border-white/20 border-t-white rounded-full w-3 h-3 animate-spin"></span>
                ) : null}
                Confirm Return
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}
