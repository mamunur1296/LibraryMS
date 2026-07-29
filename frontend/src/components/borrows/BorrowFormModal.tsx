import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { borrowBookSchema, BorrowBookFormData } from "@/lib/validations/borrow.schema";
import { borrowService } from "@/lib/services/borrow.service";
import { memberService } from "@/lib/services/member.service";
import { bookService } from "@/lib/services/book.service";
import { branchService } from "@/lib/services/branch.service";
import { useEffect, useState } from "react";

interface BorrowFormModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
}

export function BorrowFormModal({ isOpen, onClose, onSuccess }: BorrowFormModalProps) {
  const [members, setMembers] = useState<any[]>([]);
  const [books, setBooks] = useState<any[]>([]);
  const [branches, setBranches] = useState<any[]>([]);
  const [availableCopies, setAvailableCopies] = useState<any[]>([]);
  const [loadingData, setLoadingData] = useState(false);

  const {
    register,
    handleSubmit,
    reset,
    watch,
    setValue,
    formState: { errors, isSubmitting },
  } = useForm<BorrowBookFormData>({
    resolver: zodResolver(borrowBookSchema),
    defaultValues: {
      borrowDays: 14
    }
  });

  const selectedBookId = watch("bookId");

  useEffect(() => {
    if (isOpen) {
      fetchInitialData();
      reset({ borrowDays: 14 });
      setAvailableCopies([]);
    }
  }, [isOpen, reset]);

  // Fetch available copies when a book is selected
  useEffect(() => {
    if (selectedBookId) {
      fetchCopiesForBook(selectedBookId);
    } else {
      setAvailableCopies([]);
    }
  }, [selectedBookId]);

  const fetchInitialData = async () => {
    setLoadingData(true);
    try {
      // Using search endpoints with large pageSize to get list for dropdowns
      const [membersData, booksData, branchesData] = await Promise.all([
        memberService.search(undefined, "Active", 1, 100),
        bookService.search(undefined, undefined, undefined, undefined, 1, 100),
        branchService.getAll(false), // active branches
      ]);
      setMembers(membersData.items || []);
      setBooks(booksData.items || []);
      setBranches(branchesData || []);
    } catch (error) {
      console.error("Failed to load form data", error);
    } finally {
      setLoadingData(false);
    }
  };

  const fetchCopiesForBook = async (bookId: string) => {
    try {
      // Import apiClient directly or add to bookService. 
      // Assuming we can fetch from apiClient for now.
      const { apiClient } = await import("@/lib/api-client");
      const response = await apiClient.get(`/api/Books/${bookId}/available-copies`);
      setAvailableCopies(response.data);
      if (response.data.length === 1) {
        setValue("bookCopyId", response.data[0].id);
      }
    } catch (error) {
      console.error("Failed to fetch copies", error);
    }
  };

  if (!isOpen) return null;

  const onSubmit = async (data: BorrowBookFormData) => {
    try {
      await borrowService.borrowBook(data);
      reset();
      onSuccess();
      onClose();
    } catch (err: any) {
      alert(err.response?.data?.message || "Failed to issue book.");
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/60 backdrop-blur-sm overflow-y-auto py-10">
      <div className="w-full max-w-lg bg-slate-900 border border-slate-700 rounded-2xl shadow-2xl overflow-hidden m-4">
        <div className="flex items-center justify-between px-6 py-4 border-b border-slate-800 bg-slate-900/50 sticky top-0 z-10">
          <h2 className="text-xl font-semibold text-white">
            Issue Book
          </h2>
          <button onClick={onClose} className="text-slate-400 hover:text-white transition-colors">
            <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <div className="p-6">
          {loadingData ? (
             <div className="flex justify-center py-8">
               <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-indigo-500"></div>
             </div>
          ) : (
            <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
              
              <div>
                <label className="block text-sm font-medium text-slate-300 mb-1">Select Member</label>
                <select
                  {...register("memberId")}
                  className="w-full px-4 py-2 bg-slate-950 border border-slate-700 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-indigo-500"
                >
                  <option value="">-- Select Member --</option>
                  {members.map(m => (
                    <option key={m.id} value={m.id}>{m.fullName} ({m.membershipNumber})</option>
                  ))}
                </select>
                {errors.memberId && <p className="text-red-400 text-xs mt-1">{errors.memberId.message}</p>}
              </div>

              <div>
                <label className="block text-sm font-medium text-slate-300 mb-1">Select Book</label>
                <select
                  {...register("bookId")}
                  className="w-full px-4 py-2 bg-slate-950 border border-slate-700 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-indigo-500"
                >
                  <option value="">-- Select Book --</option>
                  {books.map(b => (
                    <option key={b.id} value={b.id} disabled={b.availableCopies === 0}>
                      {b.title} {b.availableCopies === 0 ? "(Out of stock)" : `(${b.availableCopies} available)`}
                    </option>
                  ))}
                </select>
                {errors.bookId && <p className="text-red-400 text-xs mt-1">{errors.bookId.message}</p>}
              </div>

              <div>
                <label className="block text-sm font-medium text-slate-300 mb-1">Select Book Copy</label>
                <select
                  {...register("bookCopyId")}
                  disabled={!selectedBookId || availableCopies.length === 0}
                  className="w-full px-4 py-2 bg-slate-950 border border-slate-700 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-indigo-500 disabled:opacity-50"
                >
                  <option value="">-- Select Copy --</option>
                  {availableCopies.map(c => (
                    <option key={c.id} value={c.id}>Copy #{c.copyNumber} (Branch: {c.branchName})</option>
                  ))}
                </select>
                {errors.bookCopyId && <p className="text-red-400 text-xs mt-1">{errors.bookCopyId.message}</p>}
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-slate-300 mb-1">Borrowing Branch</label>
                  <select
                    {...register("branchId")}
                    className="w-full px-4 py-2 bg-slate-950 border border-slate-700 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-indigo-500"
                  >
                    <option value="">-- Branch --</option>
                    {branches.map(b => (
                      <option key={b.id} value={b.id}>{b.name}</option>
                    ))}
                  </select>
                  {errors.branchId && <p className="text-red-400 text-xs mt-1">{errors.branchId.message}</p>}
                </div>
                <div>
                  <label className="block text-sm font-medium text-slate-300 mb-1">Borrow Days</label>
                  <input
                    type="number"
                    {...register("borrowDays", { valueAsNumber: true })}
                    className="w-full px-4 py-2 bg-slate-950 border border-slate-700 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-indigo-500"
                  />
                  {errors.borrowDays && <p className="text-red-400 text-xs mt-1">{errors.borrowDays.message}</p>}
                </div>
              </div>

              <div className="flex justify-end space-x-3 pt-4 border-t border-slate-800">
                <button
                  type="button"
                  onClick={onClose}
                  className="px-4 py-2 rounded-lg text-sm font-medium text-slate-300 hover:text-white hover:bg-slate-800 transition-colors"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  disabled={isSubmitting}
                  className="px-4 py-2 bg-indigo-600 hover:bg-indigo-500 text-white rounded-lg text-sm font-medium transition-colors shadow-lg shadow-indigo-500/20 disabled:opacity-50 flex items-center"
                >
                  {isSubmitting ? (
                    <span className="mr-2 border-2 border-white/20 border-t-white rounded-full w-4 h-4 animate-spin"></span>
                  ) : null}
                  Issue Book
                </button>
              </div>
            </form>
          )}
        </div>
      </div>
    </div>
  );
}
