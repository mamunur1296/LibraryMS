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
    formState: { errors, isSubmitting },
  } = useForm<ReturnBookFormData>({
    resolver: zodResolver(returnBookSchema),
  });

  if (!isOpen || !borrow) return null;

  const onSubmit = async (data: ReturnBookFormData) => {
    try {
      await borrowService.returnBook({
        borrowId: borrow.id,
        notes: data.notes,
      });
      reset();
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
          <div className="mb-4 bg-slate-900/50 p-3 rounded-lg border border-slate-700">
            <p className="text-sm text-slate-300"><span className="font-medium text-slate-400">Book:</span> {borrow.bookTitle}</p>
            <p className="text-sm text-slate-300"><span className="font-medium text-slate-400">Member:</span> {borrow.memberName}</p>
            {borrow.isOverdue && (
              <div className="mt-2 p-2 bg-red-500/10 border border-red-500/20 rounded text-xs text-red-400 font-medium">
                Warning: Book is overdue. Late fines may apply.
              </div>
            )}
          </div>

          <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
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
