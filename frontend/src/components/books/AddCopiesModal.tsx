import { useState, useEffect } from "react";
import { bookService } from "@/lib/services/book.service";
import { branchService } from "@/lib/services/branch.service";
import { BookDto } from "@/types/book.types";
import { BranchDto } from "@/types/branch.types";
import { toast } from "@/components/ui/Toast";

interface AddCopiesModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
  book: BookDto | null;
}

export function AddCopiesModal({ isOpen, onClose, onSuccess, book }: AddCopiesModalProps) {
  const [branches, setBranches] = useState<BranchDto[]>([]);
  const [loadingBranches, setLoadingBranches] = useState(false);
  
  const [branchId, setBranchId] = useState("");
  const [quantity, setQuantity] = useState(1);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [errors, setErrors] = useState<Record<string, string>>({});

  useEffect(() => {
    if (isOpen) {
      setBranchId("");
      setQuantity(1);
      setErrors({});
      fetchBranches();
    }
  }, [isOpen]);

  const fetchBranches = async () => {
    setLoadingBranches(true);
    try {
      const branchesData = await branchService.getAll(false);
      setBranches(branchesData);
      if (branchesData.length > 0) {
        setBranchId(branchesData[0].id);
      }
    } catch (error) {
      toast.error("Failed to load branches");
    } finally {
      setLoadingBranches(false);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!book) return;

    setErrors({});
    let hasError = false;
    const newErrors: Record<string, string> = {};

    if (!branchId) {
      newErrors.branchId = "Branch is required";
      hasError = true;
    }
    if (quantity < 1 || quantity > 100) {
      newErrors.quantity = "Quantity must be between 1 and 100";
      hasError = true;
    }

    if (hasError) {
      setErrors(newErrors);
      return;
    }

    setIsSubmitting(true);
    try {
      await bookService.addCopies(book.id, { branchId, quantity });
      toast.success(`${quantity} ${quantity === 1 ? 'copy' : 'copies'} added successfully!`);
      onSuccess();
      onClose();
    } catch (err: any) {
      const apiErrors = err.response?.data?.errors;
      if (apiErrors && typeof apiErrors === "object") {
        const mappedErrors: Record<string, string> = {};
        Object.keys(apiErrors).forEach((key) => {
          const fieldName = key.charAt(0).toLowerCase() + key.slice(1);
          mappedErrors[fieldName] = apiErrors[key][0];
        });
        setErrors(mappedErrors);
      } else {
        toast.error(err.response?.data?.message || "Failed to add copies.");
      }
    } finally {
      setIsSubmitting(false);
    }
  };

  if (!isOpen || !book) return null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/60 backdrop-blur-sm overflow-y-auto pt-10 pb-10">
      <div className="w-full max-w-md bg-slate-900 border border-slate-700 rounded-2xl shadow-2xl overflow-hidden m-4">
        <div className="flex items-center justify-between px-6 py-4 border-b border-slate-800 bg-slate-900/50">
          <h2 className="text-xl font-semibold text-white">
            Add Copies: {book.title}
          </h2>
          <button onClick={onClose} className="text-slate-400 hover:text-white transition-colors">
            <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <div className="p-6">
          {loadingBranches ? (
            <div className="flex justify-center items-center h-32">
              <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-indigo-500"></div>
            </div>
          ) : (
            <form onSubmit={handleSubmit} className="space-y-6">
              <div>
                <label className="block text-sm font-medium text-slate-300 mb-1">Assign to Branch</label>
                <select
                  value={branchId}
                  onChange={(e) => setBranchId(e.target.value)}
                  className="w-full px-4 py-2 bg-slate-950 border border-slate-700 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-indigo-500 appearance-none"
                >
                  <option value="">Select Branch...</option>
                  {branches.map((b) => (
                    <option key={b.id} value={b.id}>
                      {b.name}
                    </option>
                  ))}
                </select>
                {errors.branchId && <p className="text-red-400 text-xs mt-1">{errors.branchId}</p>}
              </div>

              <div>
                <label className="block text-sm font-medium text-slate-300 mb-1">Quantity</label>
                <input
                  type="number"
                  min="1"
                  max="100"
                  value={quantity}
                  onChange={(e) => setQuantity(parseInt(e.target.value) || 0)}
                  className="w-full px-4 py-2 bg-slate-950 border border-slate-700 rounded-lg text-white focus:outline-none focus:ring-2 focus:ring-indigo-500"
                />
                {errors.quantity && <p className="text-red-400 text-xs mt-1">{errors.quantity}</p>}
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
                  Add Copies
                </button>
              </div>
            </form>
          )}
        </div>
      </div>
    </div>
  );
}
